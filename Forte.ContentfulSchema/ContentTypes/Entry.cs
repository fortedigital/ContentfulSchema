using System.ComponentModel.DataAnnotations;
using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.ContentTypes
{
    public class Entry : IEntry
    {
        public string Id => this.Sys.Id;        
        public SystemProperties Sys { get;set;}

        [Display(Order = 10)]
        [ContentTypeDisplayField]
        public string Title {get;set;}
    }
}