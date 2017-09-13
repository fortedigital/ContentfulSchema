using Contentful.Core.Models;
using Forte.ContentfulSchema.Discovery;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Forte.ContentfulSchema.Core
{
    public class ContentSchemaGenerator
    {
        private readonly IContentFieldTypeProvider _contentFieldTypeProvider;

        public ContentSchemaGenerator(IContentFieldTypeProvider contentFieldTypeProvider)
        {
            this._contentFieldTypeProvider = contentFieldTypeProvider;
        }

        public IList<ContentType> GenerateContentSchema(ContentTree contentTree)
        {
            var inferedContentTypes = new List<ContentType>();

            foreach (var root in contentTree.Roots)
            {
                inferedContentTypes.AddRange(BuildTypesForBranch(root));
            }

            return inferedContentTypes;
        }

        private IList<ContentType> BuildTypesForBranch(ContentNode node)
        {
            var types = new List<ContentType>();

            types.Add(BuildContentType(node));
            foreach (var child in node.Children)
            {
                types.AddRange(BuildTypesForBranch(child));
            }

            return types;
        }

        private ContentType BuildContentType(ContentNode node)
        {
            var fieldBuilder = new ContentFieldBuilder(_contentFieldTypeProvider);

            var contentType = new ContentType
            {
                Name = node.ClrType.Name,
                SystemProperties = new SystemProperties { Id = node.ContentTypeId },
                Fields = new List<Field>()
            };

            var inferedFields = node.ClrType.GetProperties()
                                    .Where(IsContentTypeProperty)
                                    .OrderBy(p => p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Order ?? 0)
                                    .ToList();

            foreach (var field in inferedFields)
            {
                contentType.Fields.Add(fieldBuilder.BuildContentField(field));
            }

            return contentType;
        }

        private static bool IsContentTypeProperty(PropertyInfo p)
        {
            return p.PropertyType != typeof(SystemProperties) && p.SetMethod != null;
        }
    }
}
