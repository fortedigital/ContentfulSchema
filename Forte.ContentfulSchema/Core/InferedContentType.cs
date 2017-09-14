using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.Core
{
    [Obsolete("Use ContentSchemaGenerator that does not depend on this class")]
    public class InferedContentType
    {
        public string ContentTypeId { get; set; }
        public string Name => this.Type.Name;
        public Type Type { get; set; }
        public IReadOnlyCollection<InferedContentTypeField> Fields { get; set; }

        public string DisplayField => this.Fields
            .Where(f => f.Property.PropertyType == typeof(string) && CustomAttributeExtensions
                            .GetCustomAttributes<ContentTypeDisplayFieldAttribute>((MemberInfo) f.Property).Any())
            .Select(f => f.FieldId)
            .FirstOrDefault();

        public ContentType ConvertToContentType()
        {
            return new ContentType()
            {
                SystemProperties = new SystemProperties()
                {
                    Id = ContentTypeId,
                },
                Name = Name,
                DisplayField = DisplayField,
                Fields = Fields
                    .Select(f =>
                    {
                        var field = new Field()
                        {
                            Id = f.FieldId,
                            Name = f.Name,
                            Type = f.FieldType,
                            Validations = new List<IFieldValidator>()
                        };

                        if (f.IsObsolete)
                        {
                            field.Disabled = true;
                            field.Omitted = true;
                        }

                        field.LinkType = f.LinkType;
                        field.Items = f.FieldItems;

                        return field;
                    }).ToList()
            };
        }
    }
}