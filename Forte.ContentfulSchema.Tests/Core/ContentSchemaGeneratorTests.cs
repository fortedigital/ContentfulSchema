using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Core;
using Forte.ContentfulSchema.Discovery;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Core
{
    public class ContentSchemaGeneratorTests
    {
        private const string SampleContentId = "sample-content";
        private const string ParentContentId = "parent-content";
        private const string ChildContentId = "child-content";
        private const string GrandChildContentId = "grand-child-content";

        private readonly Mock<IContentFieldTypeProvider> fieldTypeProviderMock = new Mock<IContentFieldTypeProvider>();
        private readonly Mock<IContentEditorControlProvider> editorControlProviderMock = new Mock<IContentEditorControlProvider>();

        [Fact]
        public void ShouldCreateFieldAndControlDefinitionForContentProperty()
        {
            var contentTreeMock = new Mock<IContentTree>();
            contentTreeMock.Setup(m => m.Roots)
                           .Returns(new List<IContentNode>
                           {
                               new ContentNode { ClrType = typeof(SampleContent), ContentTypeId = SampleContentId }
                           });

            var schemaGenerator = new ContentSchemaGenerator(fieldTypeProviderMock.Object, editorControlProviderMock.Object);

            var contentSchema = schemaGenerator.GenerateContentSchema(contentTreeMock.Object);

            Assert.Collection(contentSchema,
                schema => Assert.Equal(SampleContentId, schema.ContentType.SystemProperties.Id));

            var sampleContent = contentSchema[0];
            var propertyName = nameof(SampleContent.Body);

            Assert.Collection(sampleContent.ContentType.Fields,
                field => Assert.Equal(propertyName, field.Name));
            Assert.Collection(sampleContent.EditorInterface.Controls,
                control => Assert.Equal(Char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1), control.FieldId));
        }

        [Fact]
        public void ShouldCreateContentTypeForEachContentNodeInHierarchy()
        {
            var contentTreeMock = GetHierarchicalContentTree();
            var schemaGenerator = new ContentSchemaGenerator(fieldTypeProviderMock.Object, editorControlProviderMock.Object);

            var contentSchema = schemaGenerator.GenerateContentSchema(contentTreeMock.Object);

            Assert.Collection(contentSchema,
                schema => Assert.Equal(ParentContentId, schema.ContentType.SystemProperties.Id),
                schema => Assert.Equal(ChildContentId, schema.ContentType.SystemProperties.Id),
                schema => Assert.Equal(GrandChildContentId, schema.ContentType.SystemProperties.Id));
        }

        private Mock<IContentTree> GetHierarchicalContentTree()
        {
            var mock = new Mock<IContentTree>();

            var parentNode = new ContentNode { ClrType = typeof(ParentContent), ContentTypeId = ParentContentId};
            var childNode = new ContentNode { ClrType = typeof(ChildContent), ContentTypeId = ChildContentId };
            var grandChildNode = new ContentNode { ClrType = typeof(GrandChildContent), ContentTypeId = GrandChildContentId };

            parentNode.Children = new[] { childNode };
            childNode.Children = new[] { grandChildNode };

            mock.Setup(m => m.Roots).Returns(new[] { parentNode });

            return mock;
        }

        [ContentType(SampleContentId)]
        private class SampleContent
        {
            public string Body { get; set; }
        }

        [ContentType(ParentContentId)]
        private class ParentContent { }

        [ContentType(ChildContentId)]
        private class ChildContent : ParentContent { }

        [ContentType(GrandChildContentId)]
        private class GrandChildContent : ChildContent { }
    }
}
