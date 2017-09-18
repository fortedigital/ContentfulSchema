using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Discovery;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Forte.ContentfulSchema.Core
{
    public class ContentSchemaGenerator
    {
        private readonly IContentFieldTypeProvider _contentFieldTypeProvider;
        private readonly IContentEditorControlProvider _contentEditorControlProvider;
        private ContentFieldValidationProvider _contentFieldValidationProvider;

        public ContentSchemaGenerator(
            IContentFieldTypeProvider contentFieldTypeProvider,
            IContentEditorControlProvider contentEditorControlProvider)
        {
            _contentFieldTypeProvider = contentFieldTypeProvider;
            _contentEditorControlProvider = contentEditorControlProvider;
        }

        public IList<ContentSchema> GenerateContentSchema(IContentTree contentTree)
        {
            _contentFieldValidationProvider = new ContentFieldValidationProvider(contentTree);
            var inferedContentTypes = new List<ContentSchema>();

            foreach (var root in contentTree.Roots)
            {
                inferedContentTypes.AddRange(BuildTypesForBranch(root));
            }

            return inferedContentTypes;
        }

        private IList<ContentSchema> BuildTypesForBranch(IContentNode node)
        {
            var types = new List<ContentSchema>();

            types.Add(BuildContentType(node));
            foreach (var child in node.Children)
            {
                types.AddRange(BuildTypesForBranch(child));
            }

            return types;
        }

        private ContentSchema BuildContentType(IContentNode node)
        {
            var fieldBuilder = new ContentFieldBuilder(_contentFieldTypeProvider);
            var fieldEditorBuilder = new ContentFieldEditorBuilder(_contentEditorControlProvider);

            var contentType = new ContentType
            {
                Name = node.ClrType.Name,
                SystemProperties = new SystemProperties { Id = node.ContentTypeId },
                Fields = new List<Field>(),
            };

            var editorInterface = new EditorInterface
            {
                Controls = new List<EditorInterfaceControl>()
            };

            var inferedProperties = node.ClrType.GetProperties()
                                    .Where(IsContentTypeProperty)
                                    .OrderBy(p => p.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Order ?? 0)
                                    .ToList();

            foreach (var property in inferedProperties)
            {
                var field = fieldBuilder.BuildContentField(property);
                field.Validations = _contentFieldValidationProvider.GetValidators(property, field);

                contentType.Fields.Add(field);
                editorInterface.Controls.Add(fieldEditorBuilder.GetEditorControl(property, field));
            }

            return new ContentSchema(contentType, editorInterface);
        }

        private static bool IsContentTypeProperty(PropertyInfo p)
        {
            return p.PropertyType != typeof(SystemProperties) && p.SetMethod != null;
        }
    }
}
