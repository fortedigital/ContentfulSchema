using Contentful.Core.Models;

namespace ContentfulExt.ContentTypes
{
    public interface IEntry{
        string Id{get;}
        SystemProperties Sys{get;set;}
        string Title{get;set;}
    }
}