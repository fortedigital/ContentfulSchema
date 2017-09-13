using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Forte.ContentfulSchema.Core
{
    public interface IContentFieldTypeProvider
    {
        string GetContentfulTypeForProperty(PropertyInfo property);
    }
}
