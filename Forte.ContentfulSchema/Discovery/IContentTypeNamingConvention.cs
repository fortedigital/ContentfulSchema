using System;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IContentTypeNamingConvention
    {
        string GetContentTypeName(Type clrType);
        string GetContentTypeDescription(Type clrType);
    }
}