using System.ComponentModel.DataAnnotations;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.ContentTypes
{
    public class ContentModelBase : IContentModelBase
    {
        public string Id => this.Sys.Id;

        public SystemProperties Sys { get; set; }
    }
}