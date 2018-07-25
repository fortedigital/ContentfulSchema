using System;
using System.Collections.Generic;
using System.Reflection;
using Contentful.Core.Models.Management;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IFieldValidationProvider
    {
        IEnumerable<IFieldValidator> GetFieldValidators(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup);
        IEnumerable<IFieldValidator> GetArrayItemValidators(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup);
    }
}