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

        public async Task<ContentType> SyncContentTypes(ContentType inferedContentType, ContentType existingContentType)
        {
            if (existingContentType == null)
            {
                inferedContentType = await _contentfulManagementClient.CreateOrUpdateContentType(inferedContentType);
                await _contentfulManagementClient.ActivateContentType(inferedContentType.SystemProperties.Id,
                    inferedContentType.SystemProperties.Version.Value);
            }
            else if (_contentTypeComparer.Equals(inferedContentType, existingContentType) == false)
            {
                inferedContentType = await _contentfulManagementClient.CreateOrUpdateContentType(inferedContentType,
                    version: existingContentType?.SystemProperties.Version);
                await _contentfulManagementClient.ActivateContentType(inferedContentType.SystemProperties.Id,
                    inferedContentType.SystemProperties.Version.Value);
            }

            return inferedContentType;
        }
    }
}