using System;
using System.Collections.Generic;
using System.Text;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IContentNode
    {
        string ContentTypeId { get; set; }

        Type ClrType { get; set; }

        IContentNode Parent { get; set; }

        IReadOnlyList<IContentNode> Children { get; }

        IReadOnlyList<IContentNode> GetAllDescedants();
    }
}
