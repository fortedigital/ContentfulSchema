using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System.Reflection;

namespace Forte.ContentfulSchema.Core
{
    public class ContentFieldEditorBuilder
    {
        private IContentEditorControlProvider _provider;

        public ContentFieldEditorBuilder(IContentEditorControlProvider contentEditorControlProvider)
        {
            _provider = contentEditorControlProvider;
        }

        public EditorInterfaceControl GetEditorControl(PropertyInfo property, Field field)
        {
            var control = new EditorInterfaceControl
            {
                FieldId = field.Id,
            };

            if (_provider != null)
            {
                var widgetId = _provider.GetWidgetIdForField(property, field);
                if (!string.IsNullOrEmpty(widgetId))
                {
                    control.WidgetId = widgetId;
                }
            }

            return control;
        }
    }
}
