using Forte.ContentfulSchema.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forte.ContentfulSchema.Tests.Discovery
{
    [ContentType("base-type-one")]
    internal class BaseTypeOne { }

    [ContentType("base-type-two")]
    internal class BaseTypeTwo { }

    [ContentType("child-type-one")]
    internal class ChildTypeOne : BaseTypeOne { }

    [ContentType("child-type-two")]
    internal class ChildTypeTwo : BaseTypeTwo { }

    [ContentType("grand-child-one")]
    internal class GrandChildOne : ChildTypeOne { }

    internal class NotContentType { }

    [ContentType("customized-content-type")]
    internal class CustomizedContentType : NotContentType { }

    internal class InheritedContentType : BaseTypeOne { }
    
    [ContentType("indirectly-inherited-content-type")]
    internal class IndirectlyInheritedContentType : InheritedContentType { }
}
