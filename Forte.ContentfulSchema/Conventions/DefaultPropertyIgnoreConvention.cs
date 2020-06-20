using System;
using System.Collections.Generic;
using System.Reflection;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace Forte.ContentfulSchema.Conventions
{
    public static class DefaultPropertyIgnoreConvention
    {
        public static IEnumerable<Func<PropertyInfo, bool>> Default = new Func<PropertyInfo, bool>[]
        {
            property => property.CanWrite == false,
            property => property.Name == "Sys" && property.PropertyType == typeof(SystemProperties),
            property => property.GetCustomAttribute<JsonIgnoreAttribute>() != null,
            property => property.GetMethod.IsPrivate && property.GetCustomAttribute<JsonPropertyAttribute>() == null 
        };
    }
}