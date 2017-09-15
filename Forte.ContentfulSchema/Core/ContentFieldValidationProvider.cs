using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Discovery;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Forte.ContentfulSchema.Core
{
    public class ContentFieldValidationProvider
    {
        private readonly ContentTree contentTree;

        public ContentFieldValidationProvider(ContentTree contentTree)
        {
            this.contentTree = contentTree;
        }

        public IList<IFieldValidator> GetValidators(PropertyInfo property, Field field)
        {
            var validators = new List<IFieldValidator>();



            return validators;
        }

        private List<LinkContentTypeValidator> GetContentTypeValidators(PropertyInfo property, Field field)
        {
            var validators = new List<LinkContentTypeValidator>();

            if (field.Type == SystemFieldTypes.Link && field.LinkType == SystemLinkTypes.Entry)
            {
                
            }

            return validators;
        }
    }
}
