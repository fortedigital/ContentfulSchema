using System;
using System.Collections.Generic;
using System.Text;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Discovery;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Discovery
{
    public class ContentTreeBuilderTests
    {
        private readonly IEnumerable<Type> _testTypes;

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
            };
        }

        [Fact]
        public void ShouldFindAllRootContentTypeClasses()
        {
            var builder = new ContentTreeBuilder(_testTypes);
            var contentTree = builder.DiscoverContentStructure();

            Assert.Collection(contentTree.Roots,
                node => Assert.Equal(typeof(BaseTypeOne), node.ClrType),
                node => Assert.Equal(typeof(BaseTypeTwo), node.ClrType),
                node => Assert.Equal(typeof(CustomizedContentType), node.ClrType));
        }

        [Fact]
        public void ShouldBuildCorrectTreeForInheritedTypes()
        {
            var builder = new ContentTreeBuilder(_testTypes);
            var contentTree = builder.DiscoverContentStructure();

            Assert.Equal(3, contentTree.Roots.Count);
            var baseTypeOneNode = contentTree.Roots[0];
            var baseTypeTwoNode = contentTree.Roots[1];
            var customizedContentNode = contentTree.Roots[2];
            
            Assert.Collection(baseTypeOneNode.Children,
                child => Assert.Equal(typeof(ChildTypeOne), child.ClrType));
            Assert.Collection(baseTypeTwoNode.Children,
                child => Assert.Equal(typeof(ChildTypeTwo), child.ClrType));
            Assert.Collection(baseTypeOneNode.Children,
                child => Assert.Collection(child.Children, 
                    grandChild => Assert.Equal(typeof(GrandChildOne), grandChild.ClrType)));
            Assert.Empty(customizedContentNode.Children);
        }

        [Fact]
        public void ShouldNotAddTypeThatInheritsTypeWithContentTypeAttribute()
        {
            var builder = new ContentTreeBuilder(_testTypes);
            var contentTree = builder.DiscoverContentStructure();

            var baseTypeOneNode = contentTree.Roots[0];

            Assert.DoesNotContain(baseTypeOneNode.Children, child => child.ClrType == typeof(InheritedContentType));
        }

        [Fact]
        public void ShouldGatherContentTypeIdDisplayFieldAndDescriptionFromAttributes()
        {
            var builder = new ContentTreeBuilder(new []{ typeof(DisplayFieldContentType) });
            var contentTree = builder.DiscoverContentStructure();
            
            Assert.Collection(contentTree.Roots,
                ct =>
                {
                    Assert.Equal("display-name-content-type", ct.ContentTypeId);
                    Assert.Equal("Awesome content type", ct.Description);
                    Assert.Equal("Title", ct.DisplayField);
                });
        }

        [Fact]
        public void ShouldGetDisplayFieldFromFirstPropertyWhenDisplayFieldAttributeIsMissing()
        {
            var builder = new ContentTreeBuilder(new []{ typeof(ContentTypeWithoutDisplayFieldAttr) });
            var contentTree = builder.DiscoverContentStructure();
            
            Assert.Collection(contentTree.Roots,
                ct =>
                {
                    Assert.Equal("content-type-without-display-field-attr", ct.ContentTypeId);
                    Assert.Equal("Title", ct.DisplayField);
                });
        }

        [ContentType("display-name-content-type", Description = "Awesome content type")]
        [ContentTypeDisplayField(nameof(Title))]
        private class DisplayFieldContentType
        {
            public string Title { get; set; }
        }

        [ContentType("content-type-without-display-field-attr")]
        private class ContentTypeWithoutDisplayFieldAttr
        {
            public string Title { get; set; }
        }
    }
}