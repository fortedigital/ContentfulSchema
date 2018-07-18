using System.Collections.Generic;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.ContentTypes;
using Forte.ContentfulSchema.Core;
using Forte.ContentfulSchema.Discovery;
using Moq;
using System.Linq;
using Xunit;
using Forte.ContentfulSchema.Conventions;
using System;

namespace Forte.ContentfulSchema.Tests.Core
{
    public class ContentFieldValidationProviderTests
    {
        const string MetaTagsContentId = "metaTags";
        const string SectionContentId = "section";
        const string HeaderSectionId = "headerSection";

        [Fact]
        public void ShouldCreateValidationRuleWithOneTypeAllowedForPropertyWithSealedType()
        {
            var provider = new LinkContentTypeValidatorProvider();
            var sealedProperty = typeof(ContentClass).GetProperty(nameof(ContentClass.Meta));
            var nameLookUp = new Dictionary<Type, string>() { {sealedProperty.PropertyType, MetaTagsContentId} };
            
            var validators = provider.GetFieldValidators(sealedProperty, nameLookUp);
            var linkValidators = validators.OfType<LinkContentTypeValidator>();

            Assert.Collection(linkValidators,
                v => Assert.Collection(v.ContentTypeIds, id => Assert.Equal(MetaTagsContentId, id.ToCamelcase())));
        }

        [Fact]
        public void ShouldCreateValidationRuleWithTwoTypesAllowedWhenPropertyTypeHasOneChildType()
        {
            var provider = new LinkContentTypeValidatorProvider();
            var propertyWithChildType = typeof(ContentClass).GetProperty(nameof(ContentClass.CustomSection));
            var childType = typeof(HeaderSection);
            var nameLookUp = new Dictionary<Type, string>() {
                { propertyWithChildType.PropertyType, SectionContentId},
                { childType, HeaderSectionId}
            };

            var validators = provider.GetFieldValidators(propertyWithChildType, nameLookUp);
            var linkValidators = validators.OfType<LinkContentTypeValidator>();

            Assert.NotEmpty(linkValidators);
            Assert.Collection(linkValidators,
                v => Assert.Collection(v.ContentTypeIds,
                    id => Assert.Equal(SectionContentId, id),
                    id => Assert.Equal(HeaderSectionId, id)));
        }

        //[Fact]
        //public void ShouldCreateValidationRuleWhenTypeOfPropertyIsGenericEntryWithContentTypeParam()
        //{
        //    var contentTreeMock = new Mock<IContentTree>();
        //    contentTreeMock.Setup(m => m.GetNodeByContentTypeId(It.Is<string>(id => id == MetaTagsContentId)))
        //        .Returns(new ContentNode { ContentTypeId = MetaTagsContentId, ClrType = typeof(MetaTags) });

        //    var provider = new ContentFieldValidationProvider(contentTreeMock.Object);

        //    var validators = provider.GetValidators(
        //        typeof(ContentClass).GetProperty(nameof(ContentClass.Meta)),
        //        new Field
        //        {
        //            Type = SystemFieldTypes.Link,
        //            LinkType = SystemLinkTypes.Entry,
        //            Id = nameof(ContentClass.EntryMeta)
        //        });

        //    var linkValidators = validators.OfType<LinkContentTypeValidator>();

        //    Assert.Collection(linkValidators,
        //        v => Assert.Collection(v.ContentTypeIds, id => Assert.Equal(MetaTagsContentId, id)));
        //}

        //[Fact]
        //public void ShouldCreateValidationRuleForFieldItemsWhenTypeOfPropertyIsCollectionOfContentTypes()
        //{
        //    var contentTreeMock = new Mock<IContentTree>();
        //    contentTreeMock.Setup(m => m.GetNodeByContentTypeId(It.Is<string>(id => id == MetaTagsContentId)))
        //        .Returns(new ContentNode { ContentTypeId = MetaTagsContentId, ClrType = typeof(MetaTags) });

        //    var provider = new ContentFieldValidationProvider(contentTreeMock.Object);

        //    var itemsValidators = provider.GetItemsValidators(
        //        typeof(ContentClass).GetProperty(nameof(ContentClass.Tags)),
        //        new Field
        //        {
        //            Id = nameof(ContentClass.EntryMeta),
        //            Type = SystemFieldTypes.Array,
        //            Items = new Schema { Type = SystemFieldTypes.Link, LinkType = SystemLinkTypes.Entry },
        //        });

        //    var linkValidators = itemsValidators.OfType<LinkContentTypeValidator>();

        //    Assert.Collection(linkValidators,
        //        v => Assert.Collection(v.ContentTypeIds, id => Assert.Equal(MetaTagsContentId, id)));
        //}

        [ContentType("content-class")]
        class ContentClass
        {
            public MetaTags Meta { get; set; }
            public Section CustomSection { get; set; }
            public Entry<MetaTags> EntryMeta { get; set; }

            public List<MetaTags> Tags { get; set; }
        }

        [ContentType(MetaTagsContentId)]
        sealed class MetaTags
        {
        }

        [ContentType(SectionContentId)]
        class Section
        {
        }

        [ContentType(HeaderSectionId)]
        class HeaderSection : Section
        {
        }
    }
}