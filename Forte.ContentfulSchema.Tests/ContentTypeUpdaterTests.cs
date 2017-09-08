using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Core;
using Moq;
using Xunit;

namespace Forte.ContentfulSchema.Tests
{
    public class ContentTypeUpdaterTests
    {
        private readonly Mock<IContentfulManagementClient> _contentfulManagementClientMock;
        private readonly Mock<IEqualityComparer<ContentType>> _contentTypeComparerMock;
        private readonly InferedContentType _inferedContentType;

        public ContentTypeUpdaterTests()
        {
            _contentfulManagementClientMock = new Mock<IContentfulManagementClient>();
            _contentTypeComparerMock = new Mock<IEqualityComparer<ContentType>>();

            // TODO Should be removed after migrating to ContentType
            _inferedContentType = new InferedContentType
            {
                ContentTypeId = "sample-content-type",
                Type = typeof(SampleContentType),
                Fields = new[]
                {
                    new InferedContentTypeField
                    {
                        FieldId = "Field1",
                        Property = typeof(SampleContentType).GetProperty(nameof(SampleContentType.Title))
                    },
                    new InferedContentTypeField
                    {
                        FieldId = "Field2",
                        Property = typeof(SampleContentType).GetProperty(nameof(SampleContentType.Age))
                    },
                }
            };

            _contentfulManagementClientMock.Setup(
                    m => m.CreateOrUpdateContentTypeAsync(It.IsAny<ContentType>(), It.IsAny<string>(), It.IsAny<int?>(),
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
            await updater.SyncContentTypes(_inferedContentType, null);

            _contentfulManagementClientMock.Verify(m =>
                m.CreateOrUpdateContentTypeAsync(It.IsAny<ContentType>(), It.IsAny<string>(),
                    null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCallCreateFunctionWithContentTypeVersionForExistingContentType()
        {
            var updater = new ContentTypeUpdater(
                _contentfulManagementClientMock.Object, _contentTypeComparerMock.Object);
            await updater.SyncContentTypes(_inferedContentType,
                new ContentType {SystemProperties = new SystemProperties() {Version = 999}});

            _contentfulManagementClientMock.Verify(m =>
                m.CreateOrUpdateContentTypeAsync(It.IsAny<ContentType>(), It.IsAny<string>(),
                    999, It.IsAny<CancellationToken>()), Times.Once);
        }

        [ContentType("sample-content-type")]
        private class SampleContentType
        {
            public string Title { get; set; }
            public int Age { get; set; }
        }
    }
}