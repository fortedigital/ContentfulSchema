using Contentful.Core.Models;

namespace Forte.ContentfulSchema.ContentTypes
{
    public interface IContentModelBase
    {
        string Id { get; }

        SystemProperties Sys { get; set; }
    }
}