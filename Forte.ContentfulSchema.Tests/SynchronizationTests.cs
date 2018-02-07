using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Extensions;
using Moq;
using Xunit;

namespace Forte.ContentfulSchema.Tests
{
    public class SychronizationTests
    {
        private readonly Mock<IContentfulManagementClient> _managementClientMock;

        public SychronizationTests()
        {
            _managementClientMock = new Mock<IContentfulManagementClient>();
            _managementClientMock
                .Setup(c => c.GetContentTypes(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<ContentType>());

            _managementClientMock
                .Setup(m => m.CreateOrUpdateContentType(It.IsAny<ContentType>(), It.IsAny<string>(),
                    It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentType() {SystemProperties = new SystemProperties() {Id = "150", Version = 1}});

            _managementClientMock
                .Setup(m => m.GetEditorInterface(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EditorInterface() {Controls = new List<EditorInterfaceControl>()});
        }
        
        [Fact]
        public async Task ShouldSynchronizeWhenThereAreNoExistingContentTypes()
        {
            await _managementClientMock.Object.SyncContentTypes<SychronizationTests>();

            _managementClientMock.Verify(m => m.GetContentTypes(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once());

            _managementClientMock
                .Verify(
                    m => m.GetEditorInterface(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<CancellationToken>()),
                    Times.Exactly(5));
        }
    }

    [ContentType("content-type-mock")]
    public class ContentTypeMock
    {
    }
}