using System.Collections.Generic;

namespace Forte.ContentfulSchema.Discovery
{
    public class ContentTree : IContentTree
    {
        private readonly IReadOnlyDictionary<string, IContentNode> _contentNodes;

        public ContentTree(IEnumerable<IContentNode> roots)
        {
            Roots = new List<IContentNode>(roots);
            _contentNodes = BuildContentDictionary(roots);
        }

        public IReadOnlyList<IContentNode> Roots { get; }

        public IContentNode GetNodeByContentTypeId(string contentTypeId)
        {
            return _contentNodes[contentTypeId];
        }

        private static Dictionary<string, IContentNode> BuildContentDictionary(IEnumerable<IContentNode> roots)
        {
            var contentNodes = new Dictionary<string, IContentNode>();
            foreach (var root in roots)
            {
                contentNodes.Add(root.ContentTypeId, root);
                foreach (var descedant in root.GetAllDescedants())
                {
                    contentNodes.Add(descedant.ContentTypeId, descedant);
                }
            }

            return contentNodes;
        }
    }
}
