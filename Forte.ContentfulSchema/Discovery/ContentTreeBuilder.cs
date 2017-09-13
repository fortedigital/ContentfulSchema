using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Forte.ContentfulSchema.Tests")]
namespace Forte.ContentfulSchema.Discovery
{
    internal class ContentTreeBuilder
    {
        private readonly IEnumerable<Type> _availableContentTypes;

        public ContentTreeBuilder(IEnumerable<Type> availableTypes)
        {
            _availableContentTypes = availableTypes
                .Where(t => t.GetCustomAttributes(typeof(ContentTypeAttribute), false).Any());
        }

        public ContentTree DiscoverContentStructure()
        {
            var contentTree = new ContentTree();
            FindRootContentTypes(contentTree);

            foreach (var rootType in contentTree.Roots)
            {
                BuildBranch(rootType);
            }

            return contentTree;
        }

        private void FindRootContentTypes(ContentTree contentTree)
        {
            foreach (var type in _availableContentTypes)
            {
                ContentTypeAttribute contentTypeAttribute = GetContentTypeAttribute(type);
                if (GetContentTypeAttribute(type.BaseType) == null)
                {
                    contentTree.Roots.Add(new ContentNode
                    {
                        ClrType = type,
                        ContentTypeId = contentTypeAttribute.ContentTypeId
                    });
                }

            }
        }

        private void BuildBranch(ContentNode node)
        {
            var childrenTypes = FindChildren(node.ClrType);
            foreach (var childType in childrenTypes)
            {
                ContentTypeAttribute contentTypeAttribute = GetContentTypeAttribute(childType);
                var childNode = new ContentNode
                {
                    Parent = node,
                    ClrType = childType,
                    ContentTypeId = contentTypeAttribute.ContentTypeId
                };

                BuildBranch(childNode);
                node.Children.Add(childNode);
            }
        }

        private static ContentTypeAttribute GetContentTypeAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(ContentTypeAttribute), false)
                .OfType<ContentTypeAttribute>()
                .SingleOrDefault();
        }

        private IList<Type> FindChildren(Type baseType)
        {
            return _availableContentTypes.Where(t => t.BaseType.GUID == baseType.GUID).ToList();
        }
    }
}
