using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Contentful.Core.Models.Management;
using Contentful.Core.Models;
using Forte.ContentfulSchema.ContentTypes;

namespace Forte.ContentfulSchema.Core
{
    public class ContentFieldTypeProvider : IContentFieldTypeProvider
    {
        private List<(Func<PropertyInfo, bool> Predicate, string ContentfulType)> _typeRules = 
            new List<(Func<PropertyInfo, bool> Predicate, string ContentfulType)>();

        public ContentFieldTypeProvider()
        {
            AddRule(prop => prop.PropertyType == typeof(string), SystemFieldTypes.Symbol);
            AddRule(prop => prop.PropertyType == typeof(bool), SystemFieldTypes.Boolean);
            AddRule(prop => prop.PropertyType == typeof(DateTime), SystemFieldTypes.Date);
            AddRule(prop => prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long), SystemFieldTypes.Integer);
            AddRule(prop => prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double), SystemFieldTypes.Number);
            AddRule(prop => prop.PropertyType == typeof(Asset), SystemFieldTypes.Link);
            AddRule(prop => prop.PropertyType.IsConstructedGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof(Entry<>), SystemFieldTypes.Link);
            AddRule(prop => typeof(Entry).IsAssignableFrom(prop.PropertyType), SystemFieldTypes.Link);
            AddRule(prop => prop.PropertyType == typeof(Location), SystemFieldTypes.Location);
            AddRule(prop => prop.PropertyType.IsConstructedGenericType &&
                            typeof(IEnumerable<>).MakeGenericType(prop.PropertyType.GetGenericArguments()[0])
                                .IsAssignableFrom(prop.PropertyType), SystemFieldTypes.Array);
        }

        public string GetContentfulTypeForProperty(PropertyInfo property)
        {
            var registeredType = _typeRules.SingleOrDefault(rule => rule.Predicate(property));

            if (!string.IsNullOrEmpty(registeredType.ContentfulType))
            {
                return registeredType.ContentfulType;
            }

            return SystemFieldTypes.Symbol;
        }

        public void AddRule(Func<PropertyInfo, bool> predicate, string contentfulType)
        {
            _typeRules.Add((predicate, contentfulType));
        }
    }
}
