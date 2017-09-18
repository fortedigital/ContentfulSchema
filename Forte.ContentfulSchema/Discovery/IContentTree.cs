using System.Collections.Generic;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IContentTree
    {
        IContentNode GetNodeByContentTypeId(string contentTypeId);

        IReadOnlyList<IContentNode> Roots { get; }
    }
}
