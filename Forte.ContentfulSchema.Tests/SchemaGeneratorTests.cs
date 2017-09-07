using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;
//using Contentful.Core.Models;
using Forte.ContentfulSchema.Core;
using Xunit;

namespace Forte.ContentfulSchema.Tests
{
    public class SchemaGeneratorTests
    {
        private const string ContentTypeTextId = "text";
        private const string ComplexContentTypeId = "complex-content";
        private const string ContentTypeWithOrderedPropsId = "content-type-with-fields-order";

        [Fact]
        public void ShouldInferClassWithContentTypeAttribute()
        {
            var inferedTypes = InferContentTypes(typeof(ContentTypeText));

            Assert.Collection(inferedTypes, ct => Assert.Equal(ContentTypeTextId, ct.ContentTypeId));
            Assert.Collection(inferedTypes, ct => Assert.Equal(typeof(ContentTypeText), ct.Type));
        }

        [Fact]
        public void ShouldNotInferClassWithoutContentTypeAttribute()
        {
            var inferedTypes = InferContentTypes(typeof(OrdinaryType));
            Assert.Empty(inferedTypes);
        }

        [Fact]
        public void ShouldTreatAllPropertiesWithSetterAsContentProperties()
        {
            var inferedTypes = InferContentTypes(typeof(ComplexContentType));

            Assert.Equal(1, inferedTypes.Count);
            var contentFields = inferedTypes[0].Fields;

            Assert.Collection(contentFields, 
                f => Assert.Equal(nameof(ComplexContentType.Name), f.Property.Name),
                f => Assert.Equal(nameof(ComplexContentType.Description), f.Property.Name),
                f => Assert.Equal(nameof(ComplexContentType.CustomNumber), f.Property.Name),
                f => Assert.Equal(nameof(ComplexContentType.Image), f.Property.Name));
        }

        [Fact]
        public void ShouldHaveCorrectOrderOfContentTypeFields()
        {
            var inferedTypes = InferContentTypes(typeof(ContentTypeWithOrderedProps));
            
            Assert.Equal(1, inferedTypes.Count);
            var contentFields = inferedTypes[0].Fields;
            
            Assert.Collection(contentFields,
                f => Assert.Equal(nameof(ContentTypeWithOrderedProps.FirstProperty), f.Property.Name),
                f => Assert.Equal(nameof(ContentTypeWithOrderedProps.MiddleProperty), f.Property.Name),
                f => Assert.Equal(nameof(ContentTypeWithOrderedProps.LastProperty), f.Property.Name));
        }
        
        private static IImmutableList<InferedContentType> InferContentTypes(params Type[] types)
        {
            var generator = new SchemaGenerator();
            var result = generator.GenerateSchema(types);
            return result;
        }
        
        [ContentType(ContentTypeTextId)]
        private class ContentTypeText {}
        
        [ContentType(ComplexContentTypeId)]
        private class ComplexContentType
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int CustomNumber { get; set; }
            public Asset Image { get; set; }
        }

        [ContentType(ContentTypeWithOrderedPropsId)]
        private class ContentTypeWithOrderedProps
        {
            [Display(Order = 10)]
            public string LastProperty { get; set; }
            
            [Display(Order = 5)]
            public string MiddleProperty { get; set; }
            
            [Display(Order = 0)]
            public string FirstProperty { get; set; }
        }
        
        private class OrdinaryType {}
    }
}