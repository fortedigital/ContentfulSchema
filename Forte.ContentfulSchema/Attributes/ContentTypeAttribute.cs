using System;

namespace Forte.ContentfulSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(string contentTypeId)
        {
            this.ContentTypeId = contentTypeId;
        }
        public string ContentTypeId { get; set; }

        public string Description { get; set; }
    }
}