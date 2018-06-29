using System.Reflection;

namespace Forte.ContentfulSchema.Discovery
{
    internal interface IContentTypeFieldNamingConvention
    {
        string GetFieldId(PropertyInfo property);
        string GetFieldName(PropertyInfo property);
    }
}