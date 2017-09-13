using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Forte.ContentfulSchema.Core
{
    public class ContentSchema
    {
        public ContentSchema(ContentType contentType, EditorInterface editorInterface)
        {
            ContentType = contentType;
            EditorInterface = editorInterface;
        }

        public ContentType ContentType { get; }
        public EditorInterface EditorInterface { get; }
    }
}
