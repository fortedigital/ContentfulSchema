using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class SchemaMerger
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;
        private readonly ContentTypeUpdater _contentTypeUpdater;
        private readonly EditorInterfaceUpdater _editorInterfaceUpdater;

        public SchemaMerger(IContentfulManagementClient contentfulManagementClient)
        {
            _contentfulManagementClient = contentfulManagementClient;
            _contentTypeUpdater = new ContentTypeUpdater(contentfulManagementClient, new ContentTypeComparer(new FieldComparer()));
            _editorInterfaceUpdater = new EditorInterfaceUpdater(contentfulManagementClient);
        }
        
        public async Task MergeSchema(IEnumerable<InferedContentType> inferedTypes, IEnumerable<ContentType> existingTypes)
        {
            var matchedTypes = MatchTypes(inferedTypes, existingTypes);

            foreach (var syncItem in matchedTypes)
            {
                try
                {
                    await _contentTypeUpdater.SyncContentTypes(syncItem.Infered, syncItem.Existing);
                    await _editorInterfaceUpdater.UpdateEditorInterface(syncItem.Infered);
//                    ContentType contentType = syncItem.Infered.ConvertToContentType();
//
//                    if (syncItem.Existing == null)
//                    {
//                        contentType = await _contentfulManagementClient.CreateOrUpdateContentTypeAsync(contentType);
//                        await _contentfulManagementClient.ActivateContentTypeAsync(contentType.SystemProperties.Id,
//                            contentType.SystemProperties.Version.Value);
//                    }
//                    else if (syncItem.Infered.IsSameAs(syncItem.Existing) == false)
//                    {
//                        contentType = await _contentfulManagementClient.CreateOrUpdateContentTypeAsync(contentType,
//                            version: syncItem.Existing?.SystemProperties.Version);
//                        await _contentfulManagementClient.ActivateContentTypeAsync(contentType.SystemProperties.Id,
//                            contentType.SystemProperties.Version.Value);
//                    }


//                    var editorInterface = await _contentfulManagementClient.GetEditorInterfaceAsync(syncItem.Infered.ContentTypeId);
//
//                    bool editorInterfaceUpdated = false;
//                    foreach (var controlToSync in editorInterface.Controls.Join(syncItem.Infered.Fields, c => c.FieldId,
//                        f => f.FieldId, (c, f) => new {Control = c, Field = f}))
//                    {
//                        if (controlToSync.Field.FieldId == "slug" && controlToSync.Control.WidgetId != "slugEditor")
//                        {
//                            controlToSync.Control.WidgetId = "slugEditor";
//                            editorInterfaceUpdated = true;
//                        }
//
//                        if (controlToSync.Field.Property.PropertyType.IsAssignableFrom(typeof(ILongString)) &&
//                            controlToSync.Control.WidgetId != "multipleLine")
//                        {
//                            controlToSync.Control.WidgetId = "multipleLine";
//                            editorInterfaceUpdated = true;
//                        }
//                    }
//
//                    if (editorInterfaceUpdated)
//                    {
//                        editorInterface = await _contentfulManagementClient.UpdateEditorInterfaceAsync(editorInterface,
//                            syncItem.Infered.ContentTypeId, editorInterface.SystemProperties.Version.Value);
//                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to update content type: {syncItem.Infered.ContentTypeId}.", e);
                }
            }            
        }

        private IEnumerable<(InferedContentType Infered, ContentType Existing)> MatchTypes(
            IEnumerable<InferedContentType> inferedTypes, IEnumerable<ContentType> existingTypes)
        {
            return inferedTypes.GroupJoin(existingTypes, t => t.ContentTypeId,
                t => t.SystemProperties.Id, 
                (i, e) => (Infered: i, Existing: e.SingleOrDefault()));
        }
    }
}