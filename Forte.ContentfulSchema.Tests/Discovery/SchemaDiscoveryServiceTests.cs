using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Conventions;
using Forte.ContentfulSchema.Core;
using Forte.ContentfulSchema.Discovery;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Discovery
{
    public class SchemaDiscoveryServiceTests
    {
        [Fact]
        public void SchemaDiscoveryServiceUsesContentTypeNamingConvention()
        {
            var mockContentTypeNamingConvention = new Mock<IContentTypeNamingConvention>();
            var typeForMock = typeof(ContentTypeWithBool);
            mockContentTypeNamingConvention.Setup(m => m.GetContentTypeDescription(typeForMock)).Returns("FakeName");
            mockContentTypeNamingConvention.Setup(m => m.GetContentTypeName(typeForMock)).Returns("FakeName");

            var service = new SchemaDiscoveryService(
                mockContentTypeNamingConvention.Object,
                new DefaultNamingConventions(),
                DefaultPropertyIgnoreConvention.Default,
                ContentTypeFieldTypeConvention.Default,
                DefaultFieldControlConvention.Default,
                new[] { new LinkContentTypeValidatorProvider() });
            var schema = service.DiscoverSchema(new [] { typeof(ContentTypeWithBool) });

            var name = schema.ContentTypeLookup.First().Value.InferedContentType.Name;
            Assert.Equal("FakeName",name);
        }

        [Fact]
        public void SchemaDiscoveryServiceUsesContentTypeFieldNamingConvention()
        {
            var mockContentTypeNamingConvention = new Mock<IContentTypeNamingConvention>();
            mockContentTypeNamingConvention.Setup(m => m.GetContentTypeDescription(It.IsAny<Type>())).Returns("FakeName");
            mockContentTypeNamingConvention.Setup(m => m.GetContentTypeName(It.IsAny<Type>())).Returns("FakeName");

            var mockContentTypeFieldNamingConvention = new Mock<IContentTypeFieldNamingConvention>();
            mockContentTypeFieldNamingConvention.Setup(m => m.GetFieldId(It.IsAny<PropertyInfo>())).Returns("FakeFieldName");
            mockContentTypeFieldNamingConvention.Setup(m => m.GetFieldName(It.IsAny<PropertyInfo>())).Returns("FakeFieldName");

            var service = new SchemaDiscoveryService(
                mockContentTypeNamingConvention.Object,
                mockContentTypeFieldNamingConvention.Object,
                DefaultPropertyIgnoreConvention.Default,
                ContentTypeFieldTypeConvention.Default,
                DefaultFieldControlConvention.Default,
                new[] { new LinkContentTypeValidatorProvider() });
            var schema = service.DiscoverSchema(new [] {typeof(ContentTypeWithBool)});

            var name = GetSchemaFirstField(schema).Name;
            Assert.Equal("FakeFieldName",name);
        }

        [Fact]
        public void SchemaDiscoveryServiceUsesContentTypePropertyIgnorePredicates()
        {
            var service = new SchemaDiscoveryService(
                new DefaultNamingConventions(),
                new DefaultNamingConventions(),
                DefaultPropertyIgnoreConvention.Default,
                ContentTypeFieldTypeConvention.Default,
                DefaultFieldControlConvention.Default,
                new[] { new LinkContentTypeValidatorProvider() });
            var schema = service.DiscoverSchema(new [] {typeof(TypeWithIgnoredProps)});

            Assert.Empty(schema.ContentTypeLookup.First().Value.InferedContentType.Fields);
        }

        [Fact]
        public void SchemaInfersObsoletePropertyAsDisabledField()
        {
            var schemaDiscoveryService = CreateDefaultDiscoveryService();
            var schemaDefinition = schemaDiscoveryService.DiscoverSchema(new[] { typeof(TypeWithObsoleteProp) });
            var isDisabled = GetSchemaFirstField(schemaDefinition).Disabled;
            
            Assert.True(isDisabled);
        }

        [Fact]
        public void SchemaInfersRequiredField()
        {
            var schemaDiscoveryService = CreateDefaultDiscoveryService();
            var schemaDefinition = schemaDiscoveryService.DiscoverSchema(new[] { typeof(TypeWithRequiredProp) });
            var isRequired = GetSchemaFirstField(schemaDefinition).Required;
            
            Assert.True(isRequired);
        }

        [Fact]
        public void SchemaInfersLocalizableField()
        {
            var schemaDiscoveryService = CreateDefaultDiscoveryService();
            var schemaDefinition = schemaDiscoveryService.DiscoverSchema(new[] { typeof(TypeWithLocalizableProp) });
            var isLocalizable = GetSchemaFirstField(schemaDefinition).Localized;
            
            Assert.True(isLocalizable);
        }

        [Fact]
        public void ShouldCreateCorrectSchemaForEmptyContentType()
        {
            var service = CreateDefaultDiscoveryService();
            var schemaDefinition = service.DiscoverSchema(new []{ typeof(EmptyContentType) });
            var inferredContentType = schemaDefinition.ContentTypeLookup[typeof(EmptyContentType)].InferedContentType;

            Assert.Empty(inferredContentType.Fields);
        }

        [Fact]
        public void ShouldNotCreateSchemaForNotContentType()
        {
            var service = CreateDefaultDiscoveryService();
            var schemaDefinition = service.DiscoverSchema(new[] {typeof(NotContentType)});
            var exists = schemaDefinition.ContentTypeLookup.ContainsKey(typeof(NotContentType));

            Assert.False(exists);
        }

        [Fact]
        public void ShouldGatherContentTypeIdDisplayFieldAndDescriptionFromAttributes()
        {
            var service = CreateDefaultDiscoveryService();
            var schema = service.DiscoverSchema(new List<Type>() { typeof(DisplayFieldContentType) });
            var typeDefinition = schema.ContentTypeLookup[typeof(DisplayFieldContentType)];

            Assert.Equal("display-name-content-type", typeDefinition.InferedContentType.SystemProperties.Id);
            Assert.Equal("Awesome content type", typeDefinition.InferedContentType.Description);
            Assert.Equal(nameof(DisplayFieldContentType.Title).ToCamelcase(), typeDefinition.InferedContentType.DisplayField);
        }

        [Fact]
        public void ShouldGetDisplayFieldFromFirstPropertyWhenDisplayFieldAttributeIsMissing()
        {
            var service = CreateDefaultDiscoveryService();
            var schema = service.DiscoverSchema(new[] { typeof(ContentTypeWithoutDisplayFieldAttr) });
            var typeDefinition = schema.ContentTypeLookup[typeof(ContentTypeWithoutDisplayFieldAttr)];

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

        private Field GetSchemaFirstField(ContentSchemaDefinition schemaDefinition)
        {
            return schemaDefinition.ContentTypeLookup.First().Value.InferedContentType.Fields.First();
        }

        private static SchemaDiscoveryService CreateDefaultDiscoveryService()
        {
            var namingConventions = new DefaultNamingConventions();

            return new SchemaDiscoveryService(
                namingConventions,
                namingConventions,
                DefaultPropertyIgnoreConvention.Default,
                ContentTypeFieldTypeConvention.Default,
                DefaultFieldControlConvention.Default,
                new[] { new LinkContentTypeValidatorProvider() }
            );
        }

        [ContentType("type-with-ignored-props")]
        private class TypeWithIgnoredProps
        {
            public string CantWrite { get; }
            public SystemProperties Sys { get; set; }
            [JsonIgnore]                                                
            public string DoNotSerialize { get; set; }
            public string PrivateGetter { private get; set; }
        }

        [ContentType("type-with-obsolete-prop")]
        private class TypeWithObsoleteProp
        {
            [Obsolete]
            public string ObsoleteProp { get; set; }
        }

        [ContentType("type-with-required-prop")]
        private class TypeWithRequiredProp
        {
            [Required]
            public string RequiredProp { get; set; }
        }

        [ContentType("type-with-localizable-prop")]
        private class TypeWithLocalizableProp
        {
            [Localizable(true)]
            public string LocalizableProp { get; set; }
        }
    }
}