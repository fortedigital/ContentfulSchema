using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Conventions;
using Forte.ContentfulSchema.Discovery;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Discovery
{
    public class SchemaDiscoveryServiceTests
    {
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

            var name = schema.ContentTypeLookup.First().Value.InferedContentType.Fields.First().Name;
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

            Assert.Equal(0,schema.ContentTypeLookup.First().Value.InferedContentType.Fields.Count);
        }

        [Fact]
        public void SchemaInfersObsoletePropertyAsDisabledField()
        {
            var schemaDiscoveryService = CreateDefaultDiscoveryService();
            var schemaDefinition = schemaDiscoveryService.DiscoverSchema(new[] { typeof(TypeWithObsoleteProp) });
            var isDisabled = schemaDefinition.ContentTypeLookup.First().Value.InferedContentType.Fields.First().Disabled;
            
            Assert.True(isDisabled);
        }

        [Fact]
        public void SchemaInfersRequiredField()
        {
            var schemaDiscoveryService = CreateDefaultDiscoveryService();
            var schemaDefinition = schemaDiscoveryService.DiscoverSchema(new[] { typeof(TypeWithRequiredProp) });
            var isRequired = schemaDefinition.ContentTypeLookup.First().Value.InferedContentType.Fields.First().Required;
            
            Assert.True(isRequired);
        }

        [Fact]
        public void SchemaInfersLocalizableField()
        {
            var schemaDiscoveryService = CreateDefaultDiscoveryService();
            var schemaDefinition = schemaDiscoveryService.DiscoverSchema(new[] { typeof(TypeWithLocalizableProp) });
            var isLocalizable = schemaDefinition.ContentTypeLookup.First().Value.InferedContentType.Fields.First().Localized;
            
            Assert.True(isLocalizable);
        }

        [Fact]
        public void ShouldCreateCorrectSchemaForEmptyContentType()
        {
            var service = CreateDefaultDiscoveryService();
            var schemaDefinition = service.DiscoverSchema(new []{ typeof(EmptyContentType) });
            var inferredContentType = schemaDefinition.ContentTypeLookup[typeof(EmptyContentType)].InferedContentType;

            Assert.Equal(0, inferredContentType.Fields.Count);
        }

        [Fact]
        public void ShouldNotCreateSchemaForNotContentType()
        {
            var service = CreateDefaultDiscoveryService();
            var schemaDefinition = service.DiscoverSchema(new[] {typeof(NotContentType)});
            var exists = schemaDefinition.ContentTypeLookup.ContainsKey(typeof(NotContentType));

            Assert.False(exists);
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