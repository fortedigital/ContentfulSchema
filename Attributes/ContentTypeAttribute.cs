using System;

namespace ContentfulExt.Attributes
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