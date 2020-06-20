using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema.Conventions
{
    public class DefaultFieldControlConvention : IFieldControlConvention
    {
        public static IFieldControlConvention Default = new DefaultFieldControlConvention(new (Func<PropertyInfo, bool> Predicate, string Widget)[]
        {
            (prop => prop.Name.Equals("slug", StringComparison.OrdinalIgnoreCase), SystemWidgetIds.SlugEditor),
            (prop => typeof(ILongString).IsAssignableFrom(prop.PropertyType), SystemWidgetIds.MultipleLine),
            (prop => typeof(IMarkdownString).IsAssignableFrom(prop.PropertyType), SystemWidgetIds.Markdown)            
        });
        
        private readonly IEnumerable<(Func<PropertyInfo, bool> Predicate, string Widget)> _conventions;

        public DefaultFieldControlConvention(IEnumerable<(Func<PropertyInfo, bool> Predicate, string Widget)> conventions)
        {
            this._conventions = conventions;
        }
               
        public string GetWidgetId(PropertyInfo property)
        {
            return this._conventions.FirstOrDefault(c => c.Predicate(property)).Widget;
        }
    }
}