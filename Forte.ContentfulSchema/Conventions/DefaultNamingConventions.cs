using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema.Conventions
{
    class DefaultNamingConventions : IContentTypeFieldNamingConvention, IContentTypeNamingConvention
    {
        public string GetFieldId(PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayAttribute>()?.Name ??
                   property.Name.ToCamelcase();
        }

        public string GetFieldName(PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayAttribute>()?.Prompt ?? property.Name;
        }

        public string GetContentTypeName(Type clrType)
        {
            return clrType.Name;
        }

        public string GetContentTypeDescription(Type clrType)
        {
            return clrType.GetCustomAttribute<ContentTypeAttribute>()?.Description;
        }
    }
}