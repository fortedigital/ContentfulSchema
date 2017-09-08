using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;
using Forte.ContentfulSchema.Core;

namespace Forte.ContentfulSchema.Extensions
{
    public static class ContentfulManagementClientExtensions
    {
        public static async Task SyncContentTypes<TApp>(this IContentfulManagementClient client)
        {
            var schemaGenerator = new SchemaGenerator();
            var schemaMerger = new SchemaMerger(client);
            
            var inferedContentTypes = schemaGenerator.GenerateSchema(typeof(TApp).GetTypeInfo().Assembly.GetTypes());
//            var inferedContentTypes =
//                typeof(TApp).GetTypeInfo().Assembly.GetTypes()
//                    .Select(t => new
//                    {
//                        Type = t,
//                        ContentTypeAttribute =
//                        t.GetTypeInfo().GetCustomAttributes<ContentTypeAttribute>().SingleOrDefault()
//                    })
//                    .Where(x => x.ContentTypeAttribute != null)
//                    .Select(x => new InferedContentType
//                    {
//                        ContentTypeId = x.ContentTypeAttribute.ContentTypeId,
//                        Type = x.Type,
//                        Fields = x.Type.GetProperties()
//                            .Where(p => IsContentTypeProperty(p))
//                            .OrderBy(p => p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Order ?? 0)
//                            .Select(p => new InferedContentTypeField
//                            {
//                                FieldId = p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Name ??
//                                          char.ToLower(p.Name[0]) + p.Name.Substring(1),
//                                Property = p
//                            })
//                            .ToList()
//                    });

            var existingContentTypes = await client.GetContentTypesAsync();
            await schemaMerger.MergeSchema(inferedContentTypes, existingContentTypes);

//            foreach (var syncItem in inferedContentTypes.GroupJoin(existingContentTypes, t => t.ContentTypeId,
//                t => t.SystemProperties.Id, (i, e) => new {Existing = e.SingleOrDefault(), Infered = i}))
//            {
//                try
//                {
//                    ContentType contentType = syncItem.Infered.ConvertToContentType(); // ContentTypeFromInferedContentType(syncItem.Infered);
//
//                    if (syncItem.Existing == null)
//                    {
//                        contentType = await client.CreateOrUpdateContentTypeAsync(contentType);
//                        await client.ActivateContentTypeAsync(contentType.SystemProperties.Id,
//                            contentType.SystemProperties.Version.Value);
//                    }
//                    else if (syncItem.Infered.IsSameAs(syncItem.Existing) == false)
//                    {
//                        contentType = await client.CreateOrUpdateContentTypeAsync(contentType,
//                            version: syncItem.Existing?.SystemProperties.Version);
//                        await client.ActivateContentTypeAsync(contentType.SystemProperties.Id,
//                            contentType.SystemProperties.Version.Value);
//                    }
//
//
//                    var editorInterface = await client.GetEditorInterfaceAsync(contentType.SystemProperties.Id);
//
//                    bool editorInterfaceUpdated = false;
//                    foreach (var controlToSync in editorInterface.Controls.Join(syncItem.Infered.Fields, c => c.FieldId,
//                        f => f.FieldId, (c, f) => new {Control = c, Field = f}))
//                    {
//                        if (controlToSync.Field.FieldId == "slug" && controlToSync.Control.WidgetId != "slugEditor")
//                        {
//                            controlToSync.Control.WidgetId = "slugEditor";
//                            editorInterfaceUpdated = true;
//                        }
//
//                        if (controlToSync.Field.Property.PropertyType.IsAssignableFrom(typeof(ILongString)) &&
//                            controlToSync.Control.WidgetId != "multipleLine")
//                        {
//                            controlToSync.Control.WidgetId = "multipleLine";
//                            editorInterfaceUpdated = true;
//                        }
//                    }
//
//                    if (editorInterfaceUpdated)
//                    {
//                        editorInterface = await client.UpdateEditorInterfaceAsync(editorInterface,
//                            contentType.SystemProperties.Id, editorInterface.SystemProperties.Version.Value);
//                    }
//                }
//                catch (Exception e)
//                {
//                    throw new Exception($"Failed to update content type: {syncItem.Infered.ContentTypeId}.", e);
//                }
//            }
        }
        
//        private static ContentType ContentTypeFromInferedContentType(InferedContentType x)
//        {
//            return new ContentType()
//            {
//                SystemProperties = new SystemProperties()
//                {
//                    Id = x.ContentTypeId,
//                },
//                Name = x.Name,
//                DisplayField = x.DisplayField,
//                Fields = x.Fields
//                    .Select(f =>
//                    {
//                        var field = new Field()
//                        {
//                            Id = f.FieldId,
//                            Name = f.Name,
//                            Type = f.FieldType,
//                            Validations = new List<IFieldValidator>()
//                        };
//
//                        if (f.IsObsolete)
//                        {
//                            field.Disabled = true;
//                            field.Omitted = true;
//                        }
//
//                        field.LinkType = f.LinkType;
//                        field.Items = f.FieldItems;
//
//                        return field;
//                    }).ToList()
//            };
//        }
    }
}