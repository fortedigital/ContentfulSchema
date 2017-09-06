using System;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core;
using System.Collections.Generic;
using Contentful.Core.Models.Management;
using ContentfulExt.Attributes;
using ContentfulExt.ContentTypes;

namespace ContentfulExt.Extensions
{
    public static class ContentfulManagementClientExtensions
    {
        public static async Task SyncContentTypes<TApp>(this IContentfulManagementClient client)
        {
            var inferedContentTypes =
                typeof(TApp).GetTypeInfo().Assembly.GetTypes()
                .Select(t => new { Type = t, ContentTypeAttribute = t.GetTypeInfo().GetCustomAttributes<ContentTypeAttribute>().SingleOrDefault() })
                .Where(x => x.ContentTypeAttribute != null)
                .Select(x => new InferedContentType{
                    ContentTypeId = x.ContentTypeAttribute.ContentTypeId,
                    Type = x.Type,
                    Fields = x.Type.GetProperties()
                            .Where(p => IsContentTypeProperty(p))
                            .OrderBy(p => p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Order ?? 0)
                            .Select(p => new InferedContentTypeField
                            {
                                FieldId = p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Name ?? char.ToLower(p.Name[0]) + p.Name.Substring(1),
                                Property = p
                            })
                            .ToList()
                });

            var existingContentTypes = await client.GetContentTypesAsync();

            foreach (var syncItem in inferedContentTypes.GroupJoin(existingContentTypes, t => t.ContentTypeId, t => t.SystemProperties.Id, (i, e) => new { Existing = e.SingleOrDefault(), Infered = i }))
            {
                try
                {
                    ContentType contentType = ContentTypeFromInferedContentType(syncItem.Infered);

                    if (syncItem.Existing == null)
                    {
                        contentType = await client.CreateOrUpdateContentTypeAsync(contentType);
                        await client.ActivateContentTypeAsync(contentType.SystemProperties.Id, contentType.SystemProperties.Version.Value);
                    }
                    else if (syncItem.Infered.IsSameAs(syncItem.Existing) == false)
                    {
                        contentType = await client.CreateOrUpdateContentTypeAsync(contentType, version: syncItem.Existing?.SystemProperties.Version);
                        await client.ActivateContentTypeAsync(contentType.SystemProperties.Id, contentType.SystemProperties.Version.Value);
                    }


                    var editorInterface = await client.GetEditorInterfaceAsync(contentType.SystemProperties.Id);

                    bool editorInterfaceUpdated = false;
                    foreach(var controlToSync in editorInterface.Controls.Join(syncItem.Infered.Fields, c => c.FieldId, f => f.FieldId, (c, f) => new { Control = c, Field = f }))
                    {
                        if (controlToSync.Field.FieldId == "slug" && controlToSync.Control.WidgetId != "slugEditor")
                        {
                            controlToSync.Control.WidgetId = "slugEditor";
                            editorInterfaceUpdated = true;
                        }
                        
                        if (controlToSync.Field.Property.PropertyType.IsAssignableFrom(typeof(ILongString)) && controlToSync.Control.WidgetId != "multipleLine")
                        {
                            controlToSync.Control.WidgetId = "multipleLine";                                                
                            editorInterfaceUpdated = true;
                        }
                    }

                    if (editorInterfaceUpdated)
                    {
                        editorInterface = await client.UpdateEditorInterfaceAsync(editorInterface, contentType.SystemProperties.Id, editorInterface.SystemProperties.Version.Value);
                    }
                } 
                catch(Exception e)
                {
                    throw new Exception($"Failed to update content type: {syncItem.Infered.ContentTypeId}.", e);
                }
            }
        }

        private static bool IsContentTypeProperty(PropertyInfo p)
        {
            return p.PropertyType != typeof(SystemProperties) && p.SetMethod != null;
        }
    
        private static ContentType ContentTypeFromInferedContentType(InferedContentType x)
        {
            return new ContentType()
            {
                SystemProperties = new SystemProperties()
                {
                    Id = x.ContentTypeId,
                },
                Name = x.Name,
                DisplayField = x.DisplayField,
                Fields = x.Fields
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

        class InferedContentType
        {
            public string ContentTypeId {get;set;}
            public string Name => this.Type.Name;
            public Type Type { get;set;}
            public IReadOnlyCollection<InferedContentTypeField> Fields { get;set;}
            public string DisplayField => this.Fields
                .Where(f => f.Property.PropertyType == typeof(string) && f.Property.GetCustomAttributes<ContentTypeDisplayFieldAttribute>().Any())
                .Select(f => f.FieldId)
                .FirstOrDefault();

            public bool IsSameAs(ContentType contentType)
            {
                if (contentType.SystemProperties.Id != this.ContentTypeId)
                    return false;
                
                if (contentType.Name != this.Name)
                    return false;
                
                if (contentType.DisplayField != this.DisplayField)
                    return false;

                if (contentType.Fields.Count != this.Fields.Count)
                    return false;

                var matchedFields = contentType.Fields
                    .GroupJoin(this.Fields, ctf => ctf.Id, ictf => ictf.FieldId, (cf, icf) => new { Field = cf, InferedField = icf.SingleOrDefault() });
                
                foreach (var fieldMatch in matchedFields)
                {
                    // Field was deleted
                    if (fieldMatch.InferedField == null)
                        return false;

                    if (fieldMatch.InferedField.IsSameAs(fieldMatch.Field) == false)
                        return false;
                }

                return true;
            }
        }

        class InferedContentTypeField{
            public string FieldId {get;set;}       
            public string FieldType => FieldTypeFromConvention(this.Property);
            public string Name => this.Property.GetCustomAttributes<DisplayAttribute>().Select(a => a.Prompt).FirstOrDefault() ?? this.Property.Name;
            public PropertyInfo Property { get;set;}
            public bool IsObsolete => this.Property.GetCustomAttributes<ObsoleteAttribute>().Any();

            public string LinkType => this.FieldType == SystemFieldTypes.Link ? this.Property.PropertyType == typeof(Asset) ? "Asset" : "Entry" : null;

            public Schema FieldItems => this.GetFieldItemsSchema();

            internal bool IsSameAs(Field field)
            {
                if (field.Id != this.FieldId)
                    return false;

                if (field.Name != this.Name)
                    return false;
                
                if (field.Type != this.FieldType)
                    return false;

                if ((field.Disabled == false || field.Omitted == false) && this.IsObsolete)
                    return false;

                if ((field.Disabled || field.Omitted) && this.IsObsolete == false)
                    return false;

                if (field.LinkType != this.LinkType)
                    return false;

                if (field.Items == null && this.FieldItems != null)
                    return false;

                if (field.Items != null)
                {
                    if (this.FieldItems == null)
                        return false;

                    if (field.Items.LinkType != this.FieldItems.LinkType)
                        return false;

                    if (field.Items.Type != this.FieldItems.Type)
                        return false;

                    if (field.Items.Validations == null && this.FieldItems.Validations != null)
                        return false; 

                    if (field.Items.Validations == null && this.FieldItems.Validations != null)
                        return false; 

                    if (field.Items.Validations != null)
                    {
                        if (this.FieldItems.Validations == null)
                            return false;
                        
                        if (field.Items.Validations.Count != this.FieldItems.Validations.Count)
                            return false;
                    }
                }

                return true;
            }

            private Schema GetFieldItemsSchema()
            {
                if (this.FieldType != SystemFieldTypes.Array)
                    return null;

                var elementType = this.Property.PropertyType.GetGenericArguments()[0];
                if (typeof(Entry).IsAssignableFrom(elementType) || (elementType.IsConstructedGenericType && elementType.GetGenericTypeDefinition() == typeof(Entry<>)))
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
                    { t => t == typeof(string), SystemFieldTypes.Symbol},
                    { t => t == typeof(bool), SystemFieldTypes.Boolean},
                    { t => t == typeof(DateTime), SystemFieldTypes.Date},
                    { t => t == typeof(int), SystemFieldTypes.Integer},
                    { t => t == typeof(float) || t == typeof(double), SystemFieldTypes.Number},                
                    { t => typeof(ILongString).IsAssignableFrom(t), SystemFieldTypes.Text},                
                    { t => typeof(IMarkdownString).IsAssignableFrom(t), SystemFieldTypes.Text},
                    { t => typeof(WrappedString).IsAssignableFrom(t), SystemFieldTypes.Symbol },                
                    { t => t == typeof(Asset), SystemFieldTypes.Link},
                    { t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(Entry<>), SystemFieldTypes.Link},
                    { t => typeof(Entry).IsAssignableFrom(t), SystemFieldTypes.Link},
                    { t => t == typeof(Location), SystemFieldTypes.Location},
                    { t => t.IsConstructedGenericType && typeof(IEnumerable<>).MakeGenericType(t.GetGenericArguments()[0]).IsAssignableFrom(t), SystemFieldTypes.Array},                                                 
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
}