//using Contentful.Core.Models;
//using Contentful.Core.Models.Management;
//using Forte.ContentfulSchema.Attributes;
//using Forte.ContentfulSchema.Core;
//using Moq;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Reflection;
//using Xunit;

//namespace Forte.ContentfulSchema.Tests.Core
//{
//    public class ContentFieldBuilderTests
//    {
//        [Fact]
//        public void ShouldCreateCorrectFieldDefinitionForStringProperty()
//        {
//            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Symbol);
//            var result = builder.BuildContentField(typeof(TestContent).GetProperty(nameof(TestContent.SampleText)));

//            Assert.Equal(SystemFieldTypes.Symbol, result.Type);
//            Assert.Equal("sampleText", result.Id);
//            Assert.Equal(nameof(TestContent.SampleText), result.Name);
//        }

//        [Theory]
//        [MemberData(nameof(GetLinkProperties))]
//        public void ShouldSetCorrectLinkTypeForLinkProperty(PropertyInfo property, string expectedLinkType)
//        {
//            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Link);
//            var result = builder.BuildContentField(property);

//            Assert.Equal(expectedLinkType, result.LinkType);
//        }

//        [Theory]
//        [MemberData(nameof(GetArrayProperties))]
//        public void ShouldSetCorrentItemsPropertyForArrayType(PropertyInfo property, string expectedItemsType)
//        {
//            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Array);
//            var result = builder.BuildContentField(property);

//            Assert.NotNull(result.Items);
//            Assert.Equal(expectedItemsType, result.Items.Type);
//        }

//        [Fact]
//        public void ShouldSetItemsLinkTypeToAssetForPropertyOfAssetCollectionType()
//        {
//            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Array);
//            var property = typeof(TestContent).GetProperty(nameof(TestContent.AssetArray));
//            var result = builder.BuildContentField(property);
            
//            Assert.NotNull(result?.Items);
//            Assert.Equal(SystemLinkTypes.Asset, result.Items.LinkType);
//        }

//        [Fact]
//        public void ShouldSetItemsLinkTypeToEntryForPropertyOfContentTypeCollectionType()
//        {
//            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Array);
//            var property = typeof(TestContent).GetProperty(nameof(TestContent.SectionContentArray));
//            var result = builder.BuildContentField(property);
            
//            Assert.NotNull(result?.Items);
//            Assert.Equal(SystemLinkTypes.Entry, result.Items.LinkType);
//        }

//        [Fact]
//        public void ShouldCreateLocalizableFieldWhenLocalizableAttribueIsSetTrueOnField()
//        {
//            ContentFieldBuilder builder = CreateBuilder(SystemFieldTypes.Symbol);
            
//            var titleField = builder.BuildContentField(
//                typeof(TestLocalizable).GetProperty(nameof(TestLocalizable.Title)));
//            var bodyField = builder.BuildContentField(
//                typeof(TestLocalizable).GetProperty(nameof(TestLocalizable.Body)));
//            var descField = builder.BuildContentField(
//                typeof(TestLocalizable).GetProperty(nameof(TestLocalizable.Description)));
            
//            Assert.True(titleField.Localized);
//            Assert.False(bodyField.Localized);
//            Assert.False(descField.Localized);
//        }

//        private static ContentFieldBuilder CreateBuilder(string providerResponse)
//        {
//            var provider = new Mock<IContentFieldTypeProvider>();
//            provider.Setup(m => m.GetContentfulTypeForProperty(It.IsAny<PropertyInfo>()))
//                    .Returns(providerResponse);

//            var builder = new ContentFieldBuilder(provider.Object);
//            return builder;
//        }

//        public static IEnumerable<object[]> GetLinkProperties()
//        {
//            return new[]
//            {
//                new object[] { typeof(TestContent).GetProperty(nameof(TestContent.EntryProp)), SystemLinkTypes.Entry },
//                new object[] { typeof(TestContent).GetProperty(nameof(TestContent.AssetProp)), SystemLinkTypes.Asset },
//            };
//        }

//        public static IEnumerable<object[]> GetArrayProperties => new[]
//        {
//            new object [] { typeof(TestContent).GetProperty(nameof(TestContent.StringArray)), SystemFieldTypes.Symbol },
//            new object [] { typeof(TestContent).GetProperty(nameof(TestContent.EntryArray)), SystemFieldTypes.Link },
//            new object [] { typeof(TestContent).GetProperty(nameof(TestContent.AssetArray)), SystemFieldTypes.Link },
//        };

//        [ContentType("test-content")]
//        private class TestContent
//        {
//            public string SampleText { get; set; }
//            public Entry<TestSectionContent> EntryProp { get; set; }
//            public Asset AssetProp { get; set; }

//            public IEnumerable<string> StringArray { get; set; }
//            public IEnumerable<Entry<string>> EntryArray { get; set; }
//            public IEnumerable<Asset> AssetArray { get; set; }
//            public IEnumerable<TestSectionContent> SectionContentArray { get; set; }
//        }

//        [ContentType("test-section")]
//        private class TestSectionContent { }

//        [ContentType("test-localizable")]
//        private class TestLocalizable
//        {
//            [Localizable(true)]
//            public string Title { get; set; }
            
//            [Localizable(false)] 
//            public string Body { get; set; }

//            public string Description { get; set; }
//        }
//    }
//}