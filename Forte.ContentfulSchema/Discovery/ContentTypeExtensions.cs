using System;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.Discovery
{
    public static class ContentTypeExtensions
    {
        public static bool IsContentType(this Type type)
        {
            return type.GetCustomAttributes(typeof(ContentTypeAttribute), false).Any();
        }

        public static ContentTypeAttribute GetContentType(this Type type)
        {
            if (type.IsGenericType && type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Entry<>))
            {
                var genericParam = type.GetGenericArguments().First();
                return GetContentTypeAttributeFromType(genericParam);
            }
            else
            {
                return GetContentTypeAttributeFromType(type);
            }
        }

        private static ContentTypeAttribute GetContentTypeAttributeFromType(Type type)
        {
            return type.GetCustomAttributes<ContentTypeAttribute>(false).FirstOrDefault();
        }
    }
}