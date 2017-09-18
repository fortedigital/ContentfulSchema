using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;
using System;

namespace Forte.ContentfulSchema.Core
{
    public class EditorInterfaceUpdater
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;

        public EditorInterfaceUpdater(IContentfulManagementClient contentfulManagementClient)
        {
            _contentfulManagementClient = contentfulManagementClient;
        }

        [Obsolete("Use SyncEditorInterface method")]
        public async Task UpdateEditorInterface(InferedContentType inferedContentType)
        {
            var editorInterface = await _contentfulManagementClient.GetEditorInterfaceAsync(inferedContentType.ContentTypeId);
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
                editorInterface = await _contentfulManagementClient.UpdateEditorInterfaceAsync(editorInterface,
                    inferedContentType.ContentTypeId, editorInterface.SystemProperties.Version.Value);
            }
        }

        public async Task SyncEditorInterface(ContentSchema contentSchema)
        {
            var existingEditorInterface = await _contentfulManagementClient.GetEditorInterfaceAsync(contentSchema.ContentType.SystemProperties.Id);

            var matchedInterfaceControls = MatchEditorControls(contentSchema.EditorInterface, existingEditorInterface);

            bool editorInterfaceUpdated = false;
            foreach (var controlToSync in matchedInterfaceControls)
            {
                if (controlToSync.InferedControl.WidgetId != controlToSync.ExistingControl.WidgetId)
                {
                    controlToSync.ExistingControl.WidgetId = controlToSync.InferedControl.WidgetId;
                    editorInterfaceUpdated = true;
                }
            }

            if (editorInterfaceUpdated)
            {
                existingEditorInterface = await _contentfulManagementClient.UpdateEditorInterfaceAsync(
                    existingEditorInterface,
                    contentSchema.ContentType.SystemProperties.Id, 
                    existingEditorInterface.SystemProperties.Version.Value);
            }
        }

        private IEnumerable<(EditorInterfaceControl InferedControl, EditorInterfaceControl ExistingControl)>
            MatchEditorControls(EditorInterface inferedInterface, EditorInterface existingInterface)
        {
            return inferedInterface.Controls.Join(existingInterface.Controls,
                infered => infered.FieldId,
                existing => existing.FieldId,
                (i, e) => (InferedControl: i, ExistingControl: e));
        }

        [Obsolete]
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