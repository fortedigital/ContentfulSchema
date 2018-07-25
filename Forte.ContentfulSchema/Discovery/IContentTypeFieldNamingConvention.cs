using System.Reflection;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IContentTypeFieldNamingConvention
    {
        string GetFieldId(PropertyInfo property);
        string GetFieldName(PropertyInfo property);
    }
}