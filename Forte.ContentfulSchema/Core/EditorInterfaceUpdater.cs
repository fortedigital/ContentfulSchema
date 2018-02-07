using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models.Management;

namespace Forte.ContentfulSchema.Core
{
    public class EditorInterfaceUpdater
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;

        public EditorInterfaceUpdater(IContentfulManagementClient contentfulManagementClient)
        {
            _contentfulManagementClient = contentfulManagementClient;
        }

        public async Task SyncEditorInterface(ContentSchema contentSchema)
        {
            var existingEditorInterface = await _contentfulManagementClient.GetEditorInterface(contentSchema.ContentType.SystemProperties.Id);

            var matchedInterfaceControls = MatchEditorControls(contentSchema.EditorInterface, existingEditorInterface);

            bool editorInterfaceUpdated = false;
            foreach (var controlToSync in matchedInterfaceControls)
            {
                if (IsUpdateAvailable(controlToSync.InferedControl, controlToSync.ExistingControl))
                {
                    controlToSync.ExistingControl.WidgetId = controlToSync.InferedControl.WidgetId;
                    editorInterfaceUpdated = true;
                }
            }

            if (editorInterfaceUpdated)
            {
                existingEditorInterface = await _contentfulManagementClient.UpdateEditorInterface(
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

        private static bool IsUpdateAvailable(EditorInterfaceControl infered, EditorInterfaceControl existing)
        {
            return !string.IsNullOrEmpty(infered.WidgetId) && infered.WidgetId != existing.WidgetId;
        }
    }
}