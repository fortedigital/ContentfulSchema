using System;
using System.Collections.Generic;
using System.Text;

namespace Forte.ContentfulSchema.Discovery
{
    public class ContentTree
    {
        public IList<ContentNode> Roots { get; } = new List<ContentNode>();

        //public IEnumerable<ContentNode> GetAll()
        //{
        //    foreach (var root in Roots)
        //    {

        //    }
        //}

        //private IEnumerable<ContentNode> GetAllChildren(ContentNode parent)
        //{
        //    var allChildren = new List<ContentNode>();

        //    foreach (var child in parent.Children)
        //    {
        //        allChildren.
        //    }
        //}
    }
}
