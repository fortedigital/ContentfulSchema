using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;
using Forte.ContentfulSchema.Core;

namespace Forte.ContentfulSchema.Discovery
{
    class SchemaDiscoveryService
    {
        private readonly IContentTypeNamingConvention _contentTypeNamingConvention;
        private readonly IEnumerable<Func<PropertyInfo, bool>> _propertyIgnorePredicates;
        private readonly IContentTypeFieldNamingConvention _fieldNamingConvention;
        private readonly IContentTypeFieldTypeConvention _fieldTypeConvention;
        private readonly IEnumerable<IFieldValidationProvider> _fieldValidationProviders;
        private readonly IFieldControlConvention _fieldControlConvention;


        public SchemaDiscoveryService(IContentTypeNamingConvention contentTypeNamingConvention, IContentTypeFieldNamingConvention fieldNamingConvention, IEnumerable<Func<PropertyInfo, bool>> propertyIgnorePredicates, IContentTypeFieldTypeConvention fieldTypeConvention, IFieldControlConvention fieldControlConvention, IEnumerable<IFieldValidationProvider> fieldValidationProviders)
        {
            _contentTypeNamingConvention = contentTypeNamingConvention;
            _propertyIgnorePredicates = propertyIgnorePredicates;
            _fieldNamingConvention = fieldNamingConvention;
            _fieldTypeConvention = fieldTypeConvention;
            _fieldValidationProviders = fieldValidationProviders;
            _fieldControlConvention = fieldControlConvention;
        }

        public ContentSchemaDefinition DiscoverSchema(IEnumerable<Type> types)
        {
            var discoveredContentTypes = types
                .Select(t => new {ClrType = t, ContentTypeAttribute = t.GetCustomAttribute<ContentTypeAttribute>()})
                .Where(x => x.ContentTypeAttribute != null)
                .Select(x => (ClrType: x.ClrType, ContentTypeId: x.ContentTypeAttribute.ContentTypeId))
                .ToList();
        
            var contentTypeDefinitions = this.GetContentTypeDefinitions(discoveredContentTypes);

            return new ContentSchemaDefinition(contentTypeDefinitions);
        }

        private IReadOnlyDictionary<Type, ContentTypeDefinition> GetContentTypeDefinitions(IEnumerable<(Type ClrType, string ContentTypeId)> discoveredContentTypes)
        {
            var result = new Dictionary<Type, ContentTypeDefinition>();

            var contentTypeIdLookup = discoveredContentTypes.ToDictionary(ct => ct.ClrType, ct => ct.ContentTypeId);
            foreach (var contentType in discoveredContentTypes)
            {
                var fieldDefinitions = this.GetContentTypeFieldDefinitions(contentType.ClrType, contentTypeIdLookup).ToList();

                result.Add(contentType.ClrType, new ContentTypeDefinition(
                    new ContentType
                    {
                        SystemProperties = new SystemProperties { Id = contentType.ContentTypeId },
                        Name = this._contentTypeNamingConvention.GetContentTypeName(contentType.ClrType),
                        Description = this._contentTypeNamingConvention.GetContentTypeDescription(contentType.ClrType),
                        Fields = fieldDefinitions.Select(d => d.Field).ToList(),
                        DisplayField = (fieldDefinitions.FirstOrDefault(d => d.IsDisplay) ?? fieldDefinitions.FirstOrDefault())?.Field.Id
                    },
                    new EditorInterface
                    {
                        Controls = fieldDefinitions.Select(d => d.Control).ToList()
                    }));
            }

            return result;
        }

        private IEnumerable<FieldDefinition> GetContentTypeFieldDefinitions(Type clrType, IDictionary<Type, string> contentTypeNameLookup)
        {
            var properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            foreach (var property in properties)
            {
                if (this._propertyIgnorePredicates.Any(p => p(property)))
                    continue;

                var fieldId = this._fieldNamingConvention.GetFieldId(property);
                var fieldType = this._fieldTypeConvention.GetFieldType(property, contentTypeNameLookup);
                yield return new FieldDefinition(
                    new Field
                    {
                        Id = fieldId,
                        Name = this._fieldNamingConvention.GetFieldName(property),
                        Type = fieldType,
                        LinkType = fieldType == SystemFieldTypes.Link ? this._fieldTypeConvention.GetLinkType(property, contentTypeNameLookup) : null,
                        Items = fieldType == SystemFieldTypes.Array ? this.GetArraySchema(property, contentTypeNameLookup) : null,
                        Validations = this._fieldValidationProviders.SelectMany(p => p.GetFieldValidators(property, contentTypeNameLookup)).ToList(),
                        Disabled = property.GetCustomAttribute<ObsoleteAttribute>() != null,
                        Required = property.GetCustomAttribute<RequiredAttribute>() != null,
                        Localized = property.GetCustomAttribute<LocalizableAttribute>()?.IsLocalizable ?? false
                    },
                    new EditorInterfaceControl
                    {
                        FieldId = fieldId,
                        WidgetId = this._fieldControlConvention.GetWidgetId(property),
                        Settings = null
                    },
                    property.GetCustomAttribute<DisplayFieldAttribute>() != null);
            }
        }

        private Schema GetArraySchema(PropertyInfo property, IDictionary<Type, string> contentTypeNameLookup)
        {
            var arrayType = this._fieldTypeConvention.GetArrayType(property, contentTypeNameLookup);
            return new Schema
            {
                Type = arrayType.Type,
                LinkType = arrayType.LinkType,
                Validations = this._fieldValidationProviders.SelectMany(p => p.GetArrayItemValidators(property, contentTypeNameLookup)).ToList()
            };
        }

        private class FieldDefinition
        {
            public readonly Field Field;
            public readonly EditorInterfaceControl Control;
            public readonly bool IsDisplay;

            public FieldDefinition(Field field, EditorInterfaceControl control, bool isDisplay = false)
            {
                this.Field = field;
                this.Control = control;
                this.IsDisplay = isDisplay;
            }
        }
    }
}