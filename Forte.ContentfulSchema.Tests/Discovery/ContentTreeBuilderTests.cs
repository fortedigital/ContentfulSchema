using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Discovery;
using Forte.ContentfulSchema.Core;
using Forte.ContentfulSchema.Conventions;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Discovery
{
    public class ContentTreeBuilderTests
    {
        private readonly IEnumerable<Type> _testTypes;
        private SchemaDiscoveryService _discoveryService;

        public ContentTreeBuilderTests()
        {
            _testTypes = new[]
            {
                typeof(BaseTypeOne),
                typeof(BaseTypeTwo),
                typeof(ChildTypeOne),
                typeof(ChildTypeTwo),
                typeof(GrandChildOne),
                typeof(NotContentType),
                typeof(CustomizedContentType),
                typeof(InheritedContentType),
                typeof(IndirectlyInheritedContentType),
            };
            var validationProviders = new[]
            {
                new LinkContentTypeValidatorProvider()
            };
            var namingConventions = new DefaultNamingConventions();
            var fieldTypeConvention = ContentTypeFieldTypeConvention.Default;
            _discoveryService = new SchemaDiscoveryService(
                namingConventions,
                namingConventions,
                DefaultPropertyIgnoreConvention.Default,
                fieldTypeConvention,
                DefaultFieldControlConvention.Default,
                validationProviders);
        }

        //[Fact]
        //public void ShouldFindAllRootContentTypeClasses()
        //{
        //    var builder = new ContentTreeBuilder(_testTypes);
        //    var contentTree = builder.DiscoverContentStructure();

        //    Assert.Collection(contentTree.Roots,
        //        node => Assert.Equal(typeof(BaseTypeOne), node.ClrType),
        //        node => Assert.Equal(typeof(BaseTypeTwo), node.ClrType),
        //        node => Assert.Equal(typeof(CustomizedContentType), node.ClrType));
        //}

        //[Fact]
        //public void ShouldBuildCorrectTreeForInheritedTypes()
        //{
        //    var builder = new ContentTreeBuilder(_testTypes);
        //    var contentTree = builder.DiscoverContentStructure();

        //    Assert.Equal(3, contentTree.Roots.Count);
        //    var baseTypeOneNode = contentTree.Roots[0];
        //    var baseTypeTwoNode = contentTree.Roots[1];
        //    var customizedContentNode = contentTree.Roots[2];
            
        //    Assert.Collection(baseTypeOneNode.Children,
        //        child => Assert.Equal(typeof(ChildTypeOne), child.ClrType),
        //        child => Assert.Equal(typeof(IndirectlyInheritedContentType), child.ClrType));
        //    Assert.Collection(baseTypeTwoNode.Children,
        //        child => Assert.Equal(typeof(ChildTypeTwo), child.ClrType));
        //    Assert.Collection(baseTypeOneNode.Children,
        //        child => Assert.Collection(child.Children, 
        //            grandChild => Assert.Equal(typeof(GrandChildOne), grandChild.ClrType)),
        //        child => Assert.Empty(child.Children));
        //    Assert.Empty(customizedContentNode.Children);
        //}

        //[Fact]
        //public void ShouldNotAddTypeThatInheritsTypeWithContentTypeAttribute()
        //{
        //    var builder = new ContentTreeBuilder(_testTypes);
        //    var contentTree = builder.DiscoverContentStructure();

        //    var baseTypeOneNode = contentTree.Roots.Single(r => r.ClrType == typeof(BaseTypeOne));

        //    Assert.DoesNotContain(baseTypeOneNode.Children, child => child.ClrType == typeof(InheritedContentType));
        //}

        //[Fact]
        //public void ShouldCorrectlyDiscoverPredecessorWhenContentTypeInheritedIndirectly()
        //{
        //    var builder = new ContentTreeBuilder(_testTypes);
        //    var contentTree = builder.DiscoverContentStructure();
            
        //    var baseTypeOneNode = contentTree.GetRootOfType<BaseTypeOne>();
            
        //    Assert.Equal(2, baseTypeOneNode.Children.Count);
        //    Assert.Contains(baseTypeOneNode.Children, child => child.ClrType == typeof(IndirectlyInheritedContentType));
        //}

        [Fact]
        public void ShouldGatherContentTypeIdDisplayFieldAndDescriptionFromAttributes()
        {
            var schema = _discoveryService.DiscoverSchema(new List<Type>() { typeof(DisplayFieldContentType) });
            ContentTypeDefinition typeDefinition;
            schema.ContentTypeDefinitions.TryGetValue(typeof(DisplayFieldContentType),out typeDefinition);

            Assert.Equal("display-name-content-type", typeDefinition.InferedContentType.SystemProperties.Id);
            Assert.Equal("Awesome content type", typeDefinition.InferedContentType.Description);
            Assert.Equal(nameof(DisplayFieldContentType.Title).ToCamelcase(), typeDefinition.InferedContentType.DisplayField);
        }

        [Fact]
        public void ShouldGetDisplayFieldFromFirstPropertyWhenDisplayFieldAttributeIsMissing()// [...]WhenDescriptionIsMissing?
        {
            var schema = _discoveryService.DiscoverSchema(new[] { typeof(ContentTypeWithoutDisplayFieldAttr) });
            ContentTypeDefinition typeDefinition;
            schema.ContentTypeDefinitions.TryGetValue(typeof(ContentTypeWithoutDisplayFieldAttr), out typeDefinition);

            Assert.Equal("content-type-without-display-field-attr",typeDefinition.InferedContentType.SystemProperties.Id);
            Assert.Equal(nameof(ContentTypeWithoutDisplayFieldAttr.Title).ToCamelcase(),typeDefinition.InferedContentType.DisplayField);
        }

        [ContentType("display-name-content-type", Description = "Awesome content type")]
        private class DisplayFieldContentType
        {
            [DisplayField]
            public string Title { get; set; }
        }

        [ContentType("content-type-without-display-field-attr")]
        private class ContentTypeWithoutDisplayFieldAttr
        {
            public string Title { get; set; }
        }
    }
}