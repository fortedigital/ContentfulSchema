using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Moq;
using Xunit;

namespace Forte.ContentfulSchema.Tests
{
    public class ContentTypeUpdaterTests
    {
        private const string _sampleContentTypeId = "sample-type-id";
        private readonly Mock<IContentfulManagementClient> _contentfulManagementClientMock;
        private readonly Mock<IEqualityComparer<ContentType>> _contentTypeComparerMock;
        private readonly ContentType _sampleInferedContentType;

        public ContentTypeUpdaterTests()
        {
            _contentfulManagementClientMock = new Mock<IContentfulManagementClient>();
            _contentTypeComparerMock = new Mock<IEqualityComparer<ContentType>>();
            _sampleInferedContentType = new ContentType
            {
                SystemProperties = new SystemProperties { Id = _sampleContentTypeId}
            };

            _contentfulManagementClientMock.Setup(
                    m => m.CreateOrUpdateContentType(It.IsAny<ContentType>(), It.IsAny<string>(), It.IsAny<int?>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((ContentType ct, string spaceId, int? version, CancellationToken token) =>
                {
                    ct.SystemProperties.Version = ct.SystemProperties.Version.GetValueOrDefault(1);
                    return ct;
                });
        }

        [Fact]
        public async Task ShouldCallCreateFunctionWithNullAsVersionArgumentForNewContentType()
        {
            var updater = new ContentTypeUpdater(
                _contentfulManagementClientMock.Object, _contentTypeComparerMock.Object);

            await updater.SyncContentTypes(_sampleInferedContentType, null);

            _contentfulManagementClientMock.Verify(m =>
                m.CreateOrUpdateContentType(It.IsAny<ContentType>(), It.IsAny<string>(),
                    null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCallCreateFunctionWithContentTypeVersionForExistingContentType()
        {
            var updater = new ContentTypeUpdater(
                _contentfulManagementClientMock.Object, _contentTypeComparerMock.Object);

            var sampleExistingType = new ContentType
            {
                SystemProperties = new SystemProperties() { Id = _sampleContentTypeId, Version = 999 }
            };

            await updater.SyncContentTypes(_sampleInferedContentType, sampleExistingType);

            _contentfulManagementClientMock.Verify(m =>
                m.CreateOrUpdateContentType(It.IsAny<ContentType>(), It.IsAny<string>(),
                    999, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}