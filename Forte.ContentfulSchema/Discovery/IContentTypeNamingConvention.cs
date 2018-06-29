using System;

namespace Forte.ContentfulSchema.Discovery
{
    internal interface IContentTypeNamingConvention
    {
        string GetContentTypeName(Type clrType);
        string GetContentTypeDescription(Type clrType);
    }
}