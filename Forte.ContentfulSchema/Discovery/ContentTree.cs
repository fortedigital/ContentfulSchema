using System.Collections.Generic;

namespace Forte.ContentfulSchema.Discovery
{
    public class ContentTree
    {
        private readonly IReadOnlyDictionary<string, ContentNode> _contentNodes;

        public ContentTree(IEnumerable<ContentNode> roots)
        {
            Roots = new List<ContentNode>(roots);
            _contentNodes = BuildContentDictionary(roots);
        }

        public IReadOnlyList<ContentNode> Roots { get; }

        public ContentNode GetNodeByContentTypeId(string contentTypeId)
        {
            return _contentNodes[contentTypeId];
        }

        private static Dictionary<string, ContentNode> BuildContentDictionary(IEnumerable<ContentNode> roots)
        {
            var contentNodes = new Dictionary<string, ContentNode>();
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
