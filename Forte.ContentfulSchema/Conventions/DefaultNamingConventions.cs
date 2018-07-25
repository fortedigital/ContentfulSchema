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

    public class ContentTypeFieldTypeConvention : IContentTypeFieldTypeConvention
    {
        public static IContentTypeFieldTypeConvention Default = new ContentTypeFieldTypeConvention(new (Func<PropertyInfo, bool>, string)[]
        {
            (prop => prop.PropertyType == typeof(bool), SystemFieldTypes.Boolean),
            (prop => prop.PropertyType == typeof(string), SystemFieldTypes.Symbol),
            (prop => prop.PropertyType == typeof(DateTime), SystemFieldTypes.Date),
            (prop => prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long), SystemFieldTypes.Integer),
            (prop => prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double), SystemFieldTypes.Number),
            (prop => prop.PropertyType == typeof(Asset), SystemFieldTypes.Link),
            (prop => prop.PropertyType.IsGenericType(typeof(Entry<>)), SystemFieldTypes.Link),
            (prop => prop.PropertyType == typeof(Location), SystemFieldTypes.Location),
            (prop => prop.PropertyType.IsEnumerable(), SystemFieldTypes.Array),
            (prop => typeof(ILongString).IsAssignableFrom(prop.PropertyType), SystemFieldTypes.Text),
            (prop => typeof(IMarkdownString).IsAssignableFrom(prop.PropertyType), SystemFieldTypes.Text),
            (prop => true, SystemFieldTypes.Symbol)
        });
        
        private readonly IEnumerable<(Func<PropertyInfo, bool> Predicate, string ContentfulType)> conventions;

        public ContentTypeFieldTypeConvention(IEnumerable<(Func<PropertyInfo, bool> Predicate, string ContentfulType)> conventions)
        {
            this.conventions = conventions;
        }

        public string GetFieldType(PropertyInfo property, IDictionary<Type, string> contentTypeIdLookup)
        {
            if (IsContentType(property.PropertyType, contentTypeIdLookup))
                return SystemFieldTypes.Link;

            return this.conventions.First(c => c.Predicate(property)).ContentfulType;
        }

        public string GetLinkType(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup)
        {
            if (property.PropertyType == typeof(Asset) || property.PropertyType.IsEnumerableOf<Asset>())
            {
                return SystemLinkTypes.Asset;
            }
            else
            {
                return SystemLinkTypes.Entry;
            }            
        }

        public (string Type, string LinkType) GetArrayType(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            if (IsContentType(elementType, contentTypeNameLookup) || (elementType.IsGenericType(typeof(Entry<>))))
            {
                return (SystemFieldTypes.Link, SystemLinkTypes.Entry);
            }
            else if (elementType == typeof(Asset))
            {
                return (SystemFieldTypes.Link, SystemLinkTypes.Asset);
            }
            else
            {
                return (SystemFieldTypes.Symbol, null);
            }
        }

        private static bool IsContentType(Type type, IDictionary<Type, string> contentTypeIdLookup)
        {
            return contentTypeIdLookup.Keys.Any(type.IsAssignableFrom);
        }
    }
}