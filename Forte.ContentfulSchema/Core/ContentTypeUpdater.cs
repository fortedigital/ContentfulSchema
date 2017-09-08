using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class ContentTypeUpdater
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;
        private readonly IEqualityComparer<ContentType> _contentTypeComparer;

        public ContentTypeUpdater(
            IContentfulManagementClient contentfulManagementClient,
            IEqualityComparer<ContentType> contentTypeComparer)
        {
            _contentfulManagementClient = contentfulManagementClient;
            _contentTypeComparer = contentTypeComparer;
        }

        public async Task<ContentType> SyncContentTypes(InferedContentType inferedContentType, ContentType existingContentType)
        {
            ContentType contentType = inferedContentType.ConvertToContentType();

            if (existingContentType == null)
            {
                contentType = await _contentfulManagementClient.CreateOrUpdateContentTypeAsync(contentType);
                await _contentfulManagementClient.ActivateContentTypeAsync(contentType.SystemProperties.Id,
                    contentType.SystemProperties.Version.Value);
            }
            else if (_contentTypeComparer.Equals(contentType, existingContentType) == false)
            {
                contentType = await _contentfulManagementClient.CreateOrUpdateContentTypeAsync(contentType,
                    version: existingContentType?.SystemProperties.Version);
                await _contentfulManagementClient.ActivateContentTypeAsync(contentType.SystemProperties.Id,
                    contentType.SystemProperties.Version.Value);
            }

            return contentType;
        }
    }
}