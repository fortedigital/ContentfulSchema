using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class ContentTypeUpdater
    {
        private readonly IContentfulManagementClient _contentfulManagementClient;

        public ContentTypeUpdater(IContentfulManagementClient contentfulManagementClient)
        {
            _contentfulManagementClient = contentfulManagementClient;
        }

        public async Task SyncContentTypes(InferedContentType inferedContentType, ContentType existingContentType)
        {
            ContentType contentType = inferedContentType.ConvertToContentType();

            if (existingContentType == null)
            {
                contentType = await _contentfulManagementClient.CreateOrUpdateContentTypeAsync(contentType);
                await _contentfulManagementClient.ActivateContentTypeAsync(contentType.SystemProperties.Id,
                    contentType.SystemProperties.Version.Value);
            }
            else if (inferedContentType.IsSameAs(existingContentType) == false)
            {
                contentType = await _contentfulManagementClient.CreateOrUpdateContentTypeAsync(contentType,
                    version: existingContentType?.SystemProperties.Version);
                await _contentfulManagementClient.ActivateContentTypeAsync(contentType.SystemProperties.Id,
                    contentType.SystemProperties.Version.Value);
            }
        }
    }
}