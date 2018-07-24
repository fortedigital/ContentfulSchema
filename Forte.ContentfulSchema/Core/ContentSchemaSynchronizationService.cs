using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class ContentSchemaSynchronizationService
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;

        public ContentSchemaSynchronizationService(IContentfulManagementClient contentfulManagementClient)
        {
            this._contentfulManagementClient = contentfulManagementClient;
        }

        public async Task UpdateSchema(IEnumerable<ContentTypeDefinition> inferedDefinitions)
        {
            var existingContentTypes = await this._contentfulManagementClient.GetContentTypes();

            var matchedTypes = inferedDefinitions.GroupJoin(existingContentTypes,
                infered => infered.InferedContentType.SystemProperties.Id,
                existing => existing.SystemProperties.Id,
                (i, e) => (InferedContentTypeDefinition: i, ExistingType: e.SingleOrDefault()));

            foreach (var syncItem in matchedTypes)
            {
                try
                {
                    var contentType = await this.SyncContentType(syncItem);

                    await this.SyncEditorInterface(contentType, syncItem);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to update content type: {syncItem.InferedContentTypeDefinition.InferedContentType.SystemProperties.Id}.", e);
                }
            }
        }

        private async Task<ContentType> SyncContentType(
            (ContentTypeDefinition InferedContentTypeDefinition, ContentType ExistingContentType) syncItem)
        {
            var contentType = syncItem.ExistingContentType;
            if (contentType == null)
            {
                //contentType = await this.CreateOrUpdateContentType(syncItem.InferedContentTypeDefinition.ContentType);
            }
            else
            {
                var contenTypeModified = syncItem.InferedContentTypeDefinition.Update(contentType);

                if (contenTypeModified)
                {
                    //contentType = await this.CreateOrUpdateContentType(syncItem.ExistingContentType);
                }
            }

            return contentType;
        }

        private async Task SyncEditorInterface(ContentType contentType,
            (ContentTypeDefinition InferedContentTypeDefinition, ContentType ExistingContentType) syncItem)
        {
            var editorInterface = await this._contentfulManagementClient.GetEditorInterface(
                contentType.SystemProperties.Id);

            var editorInterfaceModified = syncItem.InferedContentTypeDefinition.Update(editorInterface);

            if (editorInterfaceModified)
            {
                /*this.contentfulManagementClient.UpdateEditorInterface(
                    editorInterface,
                    contentType.SystemProperties.Id,
                    editorInterface.SystemProperties.Version.Value)*/;
            }
        }

        private async Task<ContentType> CreateOrUpdateContentType(ContentType contentType)
        {
            contentType = await this._contentfulManagementClient.CreateOrUpdateContentType(contentType);
            
            await this._contentfulManagementClient.ActivateContentType(
                contentType.SystemProperties.Id,
                contentType.SystemProperties.Version.Value);
            
            return contentType;
        }
    }
}