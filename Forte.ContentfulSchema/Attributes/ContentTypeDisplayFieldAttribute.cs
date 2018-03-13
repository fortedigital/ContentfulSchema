using System;

namespace Forte.ContentfulSchema.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContentTypeDisplayFieldAttribute : Attribute
    {
        public string FieldName { get; }

        public ContentTypeDisplayFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}