using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Core;
using Moq;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Core
{
    public class ContentFieldBuilderTests
    {
        [Fact]
        public void ShouldCreateCorrectFieldDefinitionForStringProperty()
        {
            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Symbol);
            var result = builder.BuildContentField(typeof(TestContent).GetProperty(nameof(TestContent.SampleText)));

            Assert.Equal(SystemFieldTypes.Symbol, result.Type);
            Assert.Equal("sampleText", result.Id);
            Assert.Equal(nameof(TestContent.SampleText), result.Name);
        }

        [Theory]
        [MemberData(nameof(GetLinkProperties))]
        public void ShouldSetCorrectLinkTypeForLinkProperty(PropertyInfo property, string expectedLinkType)
        {
            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Link);
            var result = builder.BuildContentField(property);

            Assert.Equal(expectedLinkType, result.LinkType);
        }

        [Theory]
        [MemberData(nameof(GetArrayProperties))]
        public void ShouldSetCorrentItemsPropertyForArrayType(PropertyInfo property, string expectedItemsType)
        {
            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Array);
            var result = builder.BuildContentField(property);

            Assert.NotNull(result.Items);
            Assert.Equal(expectedItemsType, result.Items.Type);
        }

        private static ContentFieldBuilder CreateBuilder(string providerResponse)
        {
            var provider = new Mock<IContentFieldTypeProvider>();
            provider.Setup(m => m.GetContentfulTypeForProperty(It.IsAny<PropertyInfo>()))
                    .Returns(providerResponse);

            var builder = new ContentFieldBuilder(provider.Object);
            return builder;
        }

        public static IEnumerable<object[]> GetLinkProperties()
        {
            return new[]
            {
                new object[] { typeof(TestContent).GetProperty(nameof(TestContent.EntryProp)), SystemLinkTypes.Entry },
                new object[] { typeof(TestContent).GetProperty(nameof(TestContent.AssetProp)), SystemLinkTypes.Asset },
            };
        }

        public static IEnumerable<object[]> GetArrayProperties => new[]
        {
            new object [] { typeof(TestContent).GetProperty(nameof(TestContent.StringArray)), SystemFieldTypes.Symbol },
            new object [] { typeof(TestContent).GetProperty(nameof(TestContent.EntryArray)), SystemFieldTypes.Link },
            new object [] { typeof(TestContent).GetProperty(nameof(TestContent.AssetArray)), SystemFieldTypes.Link },
        };

        [ContentType("test-content")]
        private class TestContent
        {
            public string SampleText { get; set; }
            public Entry<TestSectionContent> EntryProp { get; set; }
            public Asset AssetProp { get; set; }

            public IEnumerable<string> StringArray { get; set; }
            public IEnumerable<Entry<string>> EntryArray { get; set; }
            public IEnumerable<Asset> AssetArray { get; set; }
        }

        [ContentType("test-section")]
        private class TestSectionContent { }
    }
}
