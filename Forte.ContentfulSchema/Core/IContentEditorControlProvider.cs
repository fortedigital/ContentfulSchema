using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Forte.ContentfulSchema.Core
{
    public interface IContentEditorControlProvider
    {
        /// <summary>
        /// Get WidgetId for the specified content field. If does not find any registered widget for the field returns null.
        /// </summary>
        /// <returns>WidgetId or null when no widgets has been registered for the specified field</returns>
        string GetWidgetIdForField(PropertyInfo property, Field field);
    }
}
