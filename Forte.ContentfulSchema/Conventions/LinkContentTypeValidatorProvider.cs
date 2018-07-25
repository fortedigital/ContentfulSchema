using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema.Conventions
{
    public class LinkContentTypeValidatorProvider : IFieldValidationProvider
    {
        public IEnumerable<IFieldValidator> GetFieldValidators(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup)
        {
            var propertyType = property.PropertyType;
            return GetValidators(propertyType, contentTypeNameLookup);
        }

        public IEnumerable<IFieldValidator> GetArrayItemValidators(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup)
        {
            var arrayElementType = property.PropertyType.GetEnumerableElementType();
            return GetValidators(arrayElementType, contentTypeNameLookup);
        }

        private static IEnumerable<IFieldValidator> GetValidators(Type propertyType, IDictionary<Type, string> contentTypeIdLookup)
        {
            var validContentTypes = contentTypeIdLookup.Where(kvp => propertyType.IsAssignableFrom(kvp.Key)).ToList();
            if (validContentTypes.Count > 0)
            {
                return new[] {new LinkContentTypeValidator(validContentTypes.Select(kvp => kvp.Value), "")};
            }

            return Enumerable.Empty<IFieldValidator>();
        }
    }
}