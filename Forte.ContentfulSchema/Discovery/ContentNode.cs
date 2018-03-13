using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forte.ContentfulSchema.Discovery
{
    internal class ContentNode : IContentNode
    {
        public string ContentTypeId { get; set; }
        
        public string DisplayField { get; set; }
        
        public string Description { get; set; }

        public Type ClrType { get; set; }

        public IContentNode Parent { get; set; }

        public IReadOnlyList<IContentNode> Children { get; set; } = new List<ContentNode>();

        public IReadOnlyList<IContentNode> GetAllDescedants()
        {
            var descedants = new List<IContentNode>(Children);

            foreach (var child in Children)
            {
                descedants.AddRange(child.GetAllDescedants());
            }

            return descedants;
        }
    }
}
