using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Forte.ContentfulSchema.Core
{
    public class ContentFieldValidationProvider
    {
        private readonly IContentTree contentTree;

        public ContentFieldValidationProvider(IContentTree contentTree)
        {
            this.contentTree = contentTree;
        }

        public List<IFieldValidator> GetValidators(PropertyInfo property, Field field)
        {
            var validators = new List<IFieldValidator>();

            var linkContentTypeValidator = GetLinkContentTypeValidator(property, field);
            if (linkContentTypeValidator != null)
            {
                validators.Add(linkContentTypeValidator);
            }

            return validators;
        }

        private LinkContentTypeValidator GetLinkContentTypeValidator(PropertyInfo property, Field field)
        {
            if (field.Type == SystemFieldTypes.Link && field.LinkType == SystemLinkTypes.Entry)
            {
                var node = contentTree.GetNodeByContentTypeId(field.Id);

                var descedants = node.GetAllDescedants();

                return new LinkContentTypeValidator(descedants.Select(d => d.ContentTypeId).Append(field.Id));
            }

            return null;
        }
    }
}
