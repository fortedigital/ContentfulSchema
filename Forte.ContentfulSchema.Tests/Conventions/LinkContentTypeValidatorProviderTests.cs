using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Conventions;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Conventions
{
    public class LinkContentTypeValidatorProviderTests
    {
        private const string MetaTagsContentId = "metaTags";
        private const string SectionContentId = "section";
        private const string HeaderSectionId = "headerSection";

        private readonly Dictionary<Type, string> _nameLookUp = new Dictionary<Type, string>() {
            {typeof(ContentClass).GetProperty(nameof(ContentClass.Meta)).PropertyType, MetaTagsContentId},
            {typeof(ContentClass).GetProperty(nameof(ContentClass.CustomSection)).PropertyType, SectionContentId},
            {typeof(ContentClass).GetProperty(nameof(ContentClass.EntryMeta)).PropertyType, MetaTagsContentId},
            {typeof(ContentClass).GetProperty(nameof(ContentClass.Tags)).PropertyType, MetaTagsContentId},
            {typeof(HeaderSection), HeaderSectionId},
        };
        private readonly LinkContentTypeValidatorProvider _validatorProvider = new LinkContentTypeValidatorProvider();

        [Fact]
        public void ShouldCreateValidationRuleWithOneTypeAllowedForPropertyWithSealedType()
        {
            var sealedProperty = typeof(ContentClass).GetProperty(nameof(ContentClass.Meta));
            var linkValidators = _validatorProvider.GetFieldValidators(sealedProperty, _nameLookUp).OfType<LinkContentTypeValidator>();

            Assert.Collection(linkValidators,
                v => Assert.Collection(v.ContentTypeIds, id => Assert.Equal(MetaTagsContentId, id.ToCamelcase())));
        }

        [Fact]
        public void ShouldCreateValidationRuleWithTwoTypesAllowedWhenPropertyTypeHasOneChildType()
        {
            var propertyWithChildType = typeof(ContentClass).GetProperty(nameof(ContentClass.CustomSection));
            var linkValidators = _validatorProvider.GetFieldValidators(propertyWithChildType, _nameLookUp).OfType<LinkContentTypeValidator>();

            Assert.NotEmpty(linkValidators);

            Assert.Collection(linkValidators, v =>
            {
                Assert.Contains(HeaderSectionId, v.ContentTypeIds);
                Assert.Contains(SectionContentId, v.ContentTypeIds);
            });
        }

        [Fact]
        public void ShouldCreateValidationRuleWhenTypeOfPropertyIsGenericEntryWithContentTypeParam()
        {
            var entryMetaProperty = typeof(ContentClass).GetProperty(nameof(ContentClass.EntryMeta));
            var linkValidators = _validatorProvider.GetFieldValidators(entryMetaProperty, _nameLookUp).OfType<LinkContentTypeValidator>();

            Assert.Collection(linkValidators,
                v => Assert.Collection(v.ContentTypeIds, id => Assert.Equal(MetaTagsContentId, id)));
        }

        [Fact]
        public void ShouldCreateValidationRuleForFieldItemsWhenTypeOfPropertyIsCollectionOfContentTypes()
        {
            var collectionTypeProperty = typeof(ContentClass).GetProperty(nameof(ContentClass.Tags));
            var linkValidators = _validatorProvider.GetFieldValidators(collectionTypeProperty, _nameLookUp)
                .OfType<LinkContentTypeValidator>();

            Assert.Collection(linkValidators,
                v => Assert.Collection(v.ContentTypeIds, id => Assert.Equal(MetaTagsContentId, id)));
        }

        [ContentType("content-class")]
        private class ContentClass
        {
            public MetaTags Meta { get; set; }
            public Section CustomSection { get; set; }
            public Entry<MetaTags> EntryMeta { get; set; }
            public List<MetaTags> Tags { get; set; }
        }

        [ContentType(MetaTagsContentId)]
        sealed class MetaTags { }

        [ContentType(SectionContentId)]
        private class Section { }

        [ContentType(HeaderSectionId)]
        private class HeaderSection : Section { }
    }
}