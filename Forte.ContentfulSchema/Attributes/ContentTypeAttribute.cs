using System;

namespace Forte.ContentfulSchema.Attributes
{
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(string contentTypeId)
        {
            this.ContentTypeId = contentTypeId;
        }
        public string ContentTypeId { get; set; }
    }
}