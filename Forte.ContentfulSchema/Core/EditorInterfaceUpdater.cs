using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;

namespace Forte.ContentfulSchema.Core
{
    public class EditorInterfaceUpdater
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;

        public EditorInterfaceUpdater(IContentfulManagementClient contentfulManagementClient)
        {
            _contentfulManagementClient = contentfulManagementClient;
        }

        public async Task UpdateEditorInterface(InferedContentType inferedContentType)
        {
            var editorInterface = await _contentfulManagementClient.GetEditorInterface(inferedContentType.ContentTypeId);
            var controlsWithFields = GetControlsWithFields(inferedContentType, editorInterface);
            
            bool editorInterfaceUpdated = false;
            foreach (var controlToSync in controlsWithFields)
            {
                if (controlToSync.Field.FieldId == "slug" && controlToSync.Control.WidgetId != "slugEditor")
                {
                    controlToSync.Control.WidgetId = "slugEditor";
                    editorInterfaceUpdated = true;
                }

                if (controlToSync.Field.Property.PropertyType.IsAssignableFrom(typeof(ILongString)) &&
                    controlToSync.Control.WidgetId != "multipleLine")
                {
                    controlToSync.Control.WidgetId = "multipleLine";
                    editorInterfaceUpdated = true;
                }
            }

            if (editorInterfaceUpdated)
            {
                editorInterface = await _contentfulManagementClient.UpdateEditorInterface(editorInterface,
                    inferedContentType.ContentTypeId, editorInterface.SystemProperties.Version.Value);
            }
        }

        private IEnumerable<(InferedContentTypeField Field, EditorInterfaceControl Control)> GetControlsWithFields(
            InferedContentType contentType, EditorInterface editorInterface)
        {
            return editorInterface.Controls.Join(contentType.Fields,
                c => c.FieldId,
                f => f.FieldId,
                (c, f) => (Field: f, Control: c));
        }
    }
}