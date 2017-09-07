using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.Core
{
    public class SchemaGenerator
    {
        public IImmutableList<InferedContentType> GenerateSchema(IEnumerable<Type> types)
        {
            ImmutableList<InferedContentType> contentTypes = types.Select(t => new
                {
                    Type = t,
                    ContentTypeAttribute =
                    t.GetTypeInfo().GetCustomAttributes<ContentTypeAttribute>().SingleOrDefault()
                })
                .Where(x => x.ContentTypeAttribute != null)
                .Select(x => new InferedContentType
                {
                    ContentTypeId = x.ContentTypeAttribute.ContentTypeId,
                    Type = x.Type,
                    Fields = x.Type.GetProperties()
                        .Where(IsContentTypeProperty)
                        .OrderBy(p => p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Order ?? 0)
                        .Select(p => new InferedContentTypeField
                        {
                            FieldId = p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Name ??
                                      char.ToLower(p.Name[0]) + p.Name.Substring(1),
                            Property = p
                        })
                        .ToList()
                }).ToImmutableList();

            return contentTypes;
        }

        private static bool IsContentTypeProperty(PropertyInfo p)
        {
            return p.PropertyType != typeof(SystemProperties) && p.SetMethod != null;
        }
    }
}