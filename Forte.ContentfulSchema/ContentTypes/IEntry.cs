using Contentful.Core.Models;

namespace Forte.ContentfulSchema.ContentTypes
{
    public interface IEntry{
        string Id{get;}
        SystemProperties Sys{get;set;}
        string Title{get;set;}
    }
}