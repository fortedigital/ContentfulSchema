using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Forte.ContentfulSchema.Discovery;
using System;

namespace Forte.ContentfulSchema.Core
{
    public class ContentFieldBuilder
    {
        private readonly IContentFieldTypeProvider contentFieldTypeProvider;

        public ContentFieldBuilder(IContentFieldTypeProvider contentFieldTypeProvider)
        {
            this.contentFieldTypeProvider = contentFieldTypeProvider;
        }

        public Field BuildContentField(PropertyInfo property)
        {
            var fieldDefinition = new Field
            {
                Id = property.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Name ??
                        char.ToLower(property.Name[0]) + property.Name.Substring(1),
                
                Name = property.GetCustomAttributes<DisplayAttribute>()
                            .Select(a => a.Prompt).FirstOrDefault() ?? property.Name,
                
                Type = contentFieldTypeProvider.GetContentfulTypeForProperty(property),
                
                Localized = property.GetCustomAttributes<LocalizableAttribute>()
                                    .FirstOrDefault()?.IsLocalizable ?? false,

                Disabled = property.GetCustomAttributes<ObsoleteAttribute>() .Any(),
            };

            if (fieldDefinition.Type == SystemFieldTypes.Link)
            {
                fieldDefinition.LinkType = GetLinkType(property);
            }

            if (fieldDefinition.Type == SystemFieldTypes.Array)
            {
                fieldDefinition.Items = GetFieldItemsSchema(property);
            }

            return fieldDefinition;
        }

        private static string GetLinkType(PropertyInfo property)
        {
            if (property.IsTypeOf<Asset>() || property.IsCollectionOf<Asset>())
            {
                return SystemLinkTypes.Asset;
            }
            else
            {
                return SystemLinkTypes.Entry;
            }
        }

        private Schema GetFieldItemsSchema(PropertyInfo property)
        {
            var elementType = property.PropertyType.GetGenericArguments()[0];
            if (elementType.IsContentType() ||
                (elementType.IsConstructedGenericType && elementType.GetGenericTypeDefinition() == typeof(Entry<>)))
            {
                return new Schema()
                {
                    Type = SystemFieldTypes.Link,
                    LinkType = GetLinkType(property),
                    Validations = new List<IFieldValidator>()
                };
            }
            else if (elementType == typeof(Asset))
            {
                return new Schema()
                {
                    Type = SystemFieldTypes.Link,
                    LinkType = GetLinkType(property),
                    Validations = new List<IFieldValidator>()
                };
            }
            else
            {
                return new Schema()
                {
                    Type = SystemFieldTypes.Symbol,
                    Validations = new List<IFieldValidator>()
                };
            }
        }
    }
}
