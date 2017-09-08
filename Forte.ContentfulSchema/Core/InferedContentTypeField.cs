using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;

namespace Forte.ContentfulSchema.Core
{
    public class InferedContentTypeField
    {
        public string FieldId { get; set; }
        public string FieldType => FieldTypeFromConvention(this.Property);

        public string Name =>
            this.Property.GetCustomAttributes<DisplayAttribute>().Select(a => a.Prompt).FirstOrDefault() ??
            this.Property.Name;

        public PropertyInfo Property { get; set; }
        public bool IsObsolete => this.Property.GetCustomAttributes<ObsoleteAttribute>().Any();

        public string LinkType => this.FieldType == SystemFieldTypes.Link
            ? this.Property.PropertyType == typeof(Asset) ? "Asset" : "Entry"
            : null;

        public Schema FieldItems => this.GetFieldItemsSchema();

//        internal bool IsSameAs(Field field)
//        {
//            if (field.Id != this.FieldId)
//                return false;
//
//            if (field.Name != this.Name)
//                return false;
//
//            if (field.Type != this.FieldType)
//                return false;
//
//            if ((field.Disabled == false || field.Omitted == false) && this.IsObsolete)
//                return false;
//
//            if ((field.Disabled || field.Omitted) && this.IsObsolete == false)
//                return false;
//
//            if (field.LinkType != this.LinkType)
//                return false;
//
//            if (field.Items == null && this.FieldItems != null)
//                return false;
//
//            if (field.Items != null)
//            {
//                if (this.FieldItems == null)
//                    return false;
//
//                if (field.Items.LinkType != this.FieldItems.LinkType)
//                    return false;
//
//                if (field.Items.Type != this.FieldItems.Type)
//                    return false;
//
//                if (field.Items.Validations == null && this.FieldItems.Validations != null)
//                    return false;
//
//                if (field.Items.Validations == null && this.FieldItems.Validations != null)
//                    return false;
//
//                if (field.Items.Validations != null)
//                {
//                    if (this.FieldItems.Validations == null)
//                        return false;
//
//                    if (field.Items.Validations.Count != this.FieldItems.Validations.Count)
//                        return false;
//                }
//            }
//
//            return true;
//        }

        private Schema GetFieldItemsSchema()
        {
            if (this.FieldType != SystemFieldTypes.Array)
                return null;

            var elementType = this.Property.PropertyType.GetGenericArguments()[0];
            if (typeof(Entry).IsAssignableFrom(elementType) ||
                (elementType.IsConstructedGenericType && elementType.GetGenericTypeDefinition() == typeof(Entry<>)))
            {
                return new Schema()
                {
                    Type = SystemFieldTypes.Link,
                    LinkType = "Entry",
                    Validations = new List<IFieldValidator>()
                };
            }
            else if (elementType == typeof(Asset))
            {
                return new Schema()
                {
                    Type = SystemFieldTypes.Link,
                    LinkType = "Asset",
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

        private static string FieldTypeFromConvention(PropertyInfo p)
        {
            var conventions = new Dictionary<Func<Type, bool>, string>
            {
                {t => t == typeof(string), SystemFieldTypes.Symbol},
                {t => t == typeof(bool), SystemFieldTypes.Boolean},
                {t => t == typeof(DateTime), SystemFieldTypes.Date},
                {t => t == typeof(int), SystemFieldTypes.Integer},
                {t => t == typeof(float) || t == typeof(double), SystemFieldTypes.Number},
                {t => typeof(ILongString).IsAssignableFrom(t), SystemFieldTypes.Text},
                {t => typeof(IMarkdownString).IsAssignableFrom(t), SystemFieldTypes.Text},
                {t => typeof(WrappedString).IsAssignableFrom(t), SystemFieldTypes.Symbol},
                {t => t == typeof(Asset), SystemFieldTypes.Link},
                {
                    t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(Entry<>),
                    SystemFieldTypes.Link
                },
                {t => typeof(Entry).IsAssignableFrom(t), SystemFieldTypes.Link},
                {t => t == typeof(Location), SystemFieldTypes.Location},
                {
                    t => t.IsConstructedGenericType && typeof(IEnumerable<>).MakeGenericType(t.GetGenericArguments()[0])
                             .IsAssignableFrom(t),
                    SystemFieldTypes.Array
                },
            };

            foreach (var convention in conventions)
            {
                if (convention.Key(p.PropertyType))
                    return convention.Value;
            }

            return SystemFieldTypes.Symbol;
        }
    }
}