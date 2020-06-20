using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.ContentTypes;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema.Conventions
{
    public class DefaultNamingConventions : IContentTypeFieldNamingConvention, IContentTypeNamingConvention
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