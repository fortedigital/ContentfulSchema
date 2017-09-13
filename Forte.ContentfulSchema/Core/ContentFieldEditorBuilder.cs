using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System.Reflection;
using System.Linq;

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
                var fieldControlProvider = _provider.ControlsMap
                                                .SingleOrDefault(provider => provider.Predicate(property, field));

                if (fieldControlProvider.Control != null)
                {
                    control.WidgetId = fieldControlProvider.Control;
                }
            }

            return control;
        }
    }
}
