using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;

namespace Forte.ContentfulSchema.Core
{
    public class ContentEditorControlProvider : IContentEditorControlProvider
    {
        private List<(Func<PropertyInfo, Field, bool> Predicate, string Control)> _controlsMap = new List<(Func<PropertyInfo, Field, bool> Predicate, string Control)>();

        public ContentEditorControlProvider()
        {
            AddRule((prop, field) => field.Id == "slug", SystemWidgetIds.SlugEditor);
            AddRule((prop, field) => prop.PropertyType.IsAssignableFrom(typeof(ILongString)), SystemWidgetIds.MultipleLine);
        }

        public List<(Func<PropertyInfo, Field, bool> Predicate, string Control)> ControlsMap => _controlsMap;

        public void AddRule(Func<PropertyInfo, Field, bool> predicate, string widgetId)
        {
            _controlsMap.Add((Predicate: predicate, Control: widgetId ));
        }
    }
}
