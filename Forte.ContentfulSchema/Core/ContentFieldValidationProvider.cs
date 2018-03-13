using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Discovery;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public List<IFieldValidator> GetItemsValidators(PropertyInfo property, Field field)
        {
            var validators = new List<IFieldValidator>();
            
            if (field.Type == SystemFieldTypes.Array && field.Items.Type == SystemFieldTypes.Link && field.Items.LinkType == SystemLinkTypes.Entry && property.PropertyType.IsGenericType)
            {
                var collectionType = property.PropertyType.GenericTypeArguments.FirstOrDefault();
                var contentTypeAttr = collectionType?.GetContentType();

                if (contentTypeAttr != null)
                {
                    var node = contentTree.GetNodeByContentTypeId(contentTypeAttr.ContentTypeId);
                    var descedants = node.GetAllDescedants();
                    
                    validators.Add(new LinkContentTypeValidator(descedants.Select(d => d.ContentTypeId)
                        .Append(contentTypeAttr.ContentTypeId)));
                }
            }

            return validators;
        }

        private LinkContentTypeValidator GetLinkContentTypeValidator(PropertyInfo property, Field field)
        {
            if (field.Type == SystemFieldTypes.Link && field.LinkType == SystemLinkTypes.Entry)
            {
                var contentTypeAttr = property.PropertyType.GetContentType();

                if (contentTypeAttr != null)
                {
                    var node = contentTree.GetNodeByContentTypeId(contentTypeAttr.ContentTypeId);
                    var descedants = node.GetAllDescedants();

                    return new LinkContentTypeValidator(descedants.Select(d => d.ContentTypeId)
                        .Append(contentTypeAttr.ContentTypeId));
                }
            }

            return null;
        }
    }
}