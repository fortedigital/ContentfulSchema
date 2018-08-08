using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
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
        public void SchemaDiscoveryServiceUsesProvidedContentTypeNamingConvention()
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
        public void SchemaDiscoveryServiceUsesProvidedContentTypeFieldNamingConvention()
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
        public void SchemaDiscoveryServiceUsesProvidedFieldControlConvention()
        {
            var expectedFieldName = typeof(ContentTypeWithBool).GetProperties().First().Name.ToCamelcase();
            var mockControlConvention = new Mock<IFieldControlConvention>();
            mockControlConvention.Setup(m => m.GetWidgetId(It.IsAny<PropertyInfo>())).Returns("FakeWidgetId");

            var service = new SchemaDiscoveryService(
                new DefaultNamingConventions(), 
                new DefaultNamingConventions(), 
                DefaultPropertyIgnoreConvention.Default,
                ContentTypeFieldTypeConvention.Default,
                mockControlConvention.Object,
                new[] { new LinkContentTypeValidatorProvider() });

            var schema = service.DiscoverSchema(new [] {typeof(ContentTypeWithBool)});
            var inferredEditorInterfaceControl = schema.ContentTypeLookup[typeof(ContentTypeWithBool)].InferedEditorInterface.Controls.First();
            var controlWidgetId = inferredEditorInterfaceControl.WidgetId;
            var controlFieldId = inferredEditorInterfaceControl.FieldId;

            Assert.Equal("FakeWidgetId", controlWidgetId);
            Assert.Equal(expectedFieldName, controlFieldId);
        }

        [Fact]
        public void SchemaDiscoveryServiceUsesContentTypePropertyIgnorePredicates()
        {
            var service = CreateDefaultDiscoveryService();
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
        public void ShouldCreateWithNoFieldsForEmptyContentType()
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
            var expectedName = nameof(DisplayFieldContentType.Title).ToCamelcase();
            var id = typeDefinition.InferedContentType.SystemProperties.Id;
            var description = typeDefinition.InferedContentType.Description;
            var displayField = typeDefinition.InferedContentType.DisplayField;

            Assert.Equal("display-name-content-type", id);
            Assert.Equal("Awesome content type", description);
            Assert.Equal(expectedName, displayField);
        }

        [Fact]
        public void ShouldGetDisplayFieldFromFirstPropertyWhenDisplayFieldAttributeIsMissing()
        {
            var service = CreateDefaultDiscoveryService();
            var schema = service.DiscoverSchema(new[] { typeof(ContentTypeWithoutDisplayFieldAttr) });
            var typeDefinition = schema.ContentTypeLookup[typeof(ContentTypeWithoutDisplayFieldAttr)];
            var expectedName = nameof(ContentTypeWithoutDisplayFieldAttr.Title).ToCamelcase();
            var id = typeDefinition.InferedContentType.SystemProperties.Id;
            var displayField = typeDefinition.InferedContentType.DisplayField;

            Assert.Equal("content-type-without-display-field-attr", id);
            Assert.Equal(expectedName, displayField);
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