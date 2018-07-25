using System;
using System.Collections.Generic;
using System.Reflection;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IContentTypeFieldTypeConvention
    {
        string GetFieldType(PropertyInfo property, IDictionary<Type, string> contentTypeIdLookup);
        string GetLinkType(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup);
        (string Type, string LinkType) GetArrayType(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup);
    }
}