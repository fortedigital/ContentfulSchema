using System;
using System.Linq;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.Discovery
{
    public static class ContentTypeExtensions
    {
        public static bool IsContentType(this Type type)
        {
            return type.GetCustomAttributes(typeof(ContentTypeAttribute), false).Any();
        }
    }
}