using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forte.ContentfulSchema.Discovery
{
    public class ContentNode
    {
        public string ContentTypeId { get; set; }

        public Type ClrType { get; set; }

        public ContentNode Parent { get; set; }

        public IList<ContentNode> Children { get; } = new List<ContentNode>();

        public IList<ContentNode> GetAllDescedants()
        {
            var descedants = new List<ContentNode>(Children);

            foreach (var child in Children)
            {
                descedants.AddRange(child.GetAllDescedants());
            }

            return descedants;
        }
    }
}
