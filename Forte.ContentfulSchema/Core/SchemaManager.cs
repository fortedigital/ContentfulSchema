using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class SchemaManager
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;
        private readonly ContentTypeUpdater _contentTypeUpdater;
        private readonly EditorInterfaceUpdater _editorInterfaceUpdater;

        public SchemaManager(IContentfulManagementClient contentfulManagementClient)
        {
            _contentfulManagementClient = contentfulManagementClient;
            _contentTypeUpdater = new ContentTypeUpdater(contentfulManagementClient, new ContentTypeComparer(new FieldComparer()));
            _editorInterfaceUpdater = new EditorInterfaceUpdater(contentfulManagementClient);
        }

        public async Task UpdateSchema(IEnumerable<ContentSchema> inferedDefinitions)
        {
            var existingContentTypes = await _contentfulManagementClient.GetContentTypesAsync();
            var matchedTypes = MatchContentTypes(inferedDefinitions, existingContentTypes);

            foreach (var syncItem in matchedTypes)
            {
                try
                {
                    await _contentTypeUpdater.SyncContentTypes(syncItem.InferedType.ContentType, syncItem.ExistingType);
                    await _editorInterfaceUpdater.SyncEditorInterface(syncItem.InferedType);

                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to update content type: {syncItem.InferedType.ContentType.SystemProperties.Id}.", e);
                }
            }

        }
        
        [Obsolete]
        public async Task MergeSchema(IEnumerable<InferedContentType> inferedTypes, IEnumerable<ContentType> existingTypes)
        {
            var matchedTypes = MatchTypes(inferedTypes, existingTypes);

            foreach (var syncItem in matchedTypes)
            {
                try
                {
                    await _contentTypeUpdater.SyncContentTypes(syncItem.Infered.ConvertToContentType(), syncItem.Existing);
                    await _editorInterfaceUpdater.UpdateEditorInterface(syncItem.Infered);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to update content type: {syncItem.Infered.ContentTypeId}.", e);
                }
            }            
        }

         private IEnumerable<(ContentSchema InferedType, ContentType ExistingType)> MatchContentTypes(
             IEnumerable<ContentSchema> inferedDefinitions, IEnumerable<ContentType> existingDefinitions)
        {
            return inferedDefinitions.GroupJoin(existingDefinitions,
                infered => infered.ContentType.SystemProperties.Id,
                existing => existing.SystemProperties.Id,
                (i, e) => (InferedType: i, ExistingType: e.SingleOrDefault()));
        }

        [Obsolete]
        private IEnumerable<(InferedContentType Infered, ContentType Existing)> MatchTypes(
            IEnumerable<InferedContentType> inferedTypes, IEnumerable<ContentType> existingTypes)
        {
            return inferedTypes.GroupJoin(existingTypes, t => t.ContentTypeId,
                t => t.SystemProperties.Id, 
                (i, e) => (Infered: i, Existing: e.SingleOrDefault()));
        }
    }
}