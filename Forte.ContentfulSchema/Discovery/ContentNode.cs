using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forte.ContentfulSchema.Discovery
{
    public class ContentNode
    {
        //public ContentType ContentfulType { get; set; }
        public string ContentTypeId { get; set; }

        public Type ClrType { get; set; }

        public ContentNode Parent { get; set; }

        public IList<ContentNode> Children { get; } = new List<ContentNode>();
    }
}
