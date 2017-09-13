using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Forte.ContentfulSchema.Core
{
    public interface IContentEditorControlProvider
    {
        List<(Func<PropertyInfo, Field, bool> Predicate, string Control)> ControlsMap { get; }
    }
}
