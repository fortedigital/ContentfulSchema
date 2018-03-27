using Contentful.Core.Models;
using Forte.ContentfulSchema.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var rootContentTypes = FindRootContentTypes();

            return new ContentTree(rootContentTypes);
        }

        private IEnumerable<ContentNode> FindRootContentTypes()
        {
            foreach (var type in _availableContentTypes)
            {
                bool hasContentTypePredecessor =
                    GetInheritancePredecessors(type).Any(t => GetContentTypeAttribute(t) != null);

                if (!hasContentTypePredecessor)
                {
                    var root = BuildContentNodeForType(type, null);
                    root.Children = GetChildren(root);
                    yield return root;
                }
            }
        }

        private static IEnumerable<Type> GetInheritancePredecessors(Type type)
        {
            for (var current = type.BaseType; current != null; current = current.BaseType)
            {
                yield return current;
            }
        }

        private List<ContentNode> GetChildren(ContentNode node)
        {
            var childrenTypes = FindChildren(node.ClrType);
            var childrenNodes = new List<ContentNode>();
            foreach (var childType in childrenTypes)
            {
                var childNode = BuildContentNodeForType(childType, node);

                childNode.Children = GetChildren(childNode);
                childrenNodes.Add(childNode);
            }

            return childrenNodes;
        }

        private static ContentNode BuildContentNodeForType(Type type, ContentNode parent)
        {
            var contentTypeAttribute = GetContentTypeAttribute(type);
            var contentNode = new ContentNode
            {
                Parent = parent,
                ClrType = type,
                ContentTypeId = contentTypeAttribute.ContentTypeId,
                DisplayField = GetDisplayField(type),
                Description = contentTypeAttribute.Description
            };
            return contentNode;
        }

        private static ContentTypeAttribute GetContentTypeAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(ContentTypeAttribute), false)
                .OfType<ContentTypeAttribute>()
                .SingleOrDefault();
        }

        private static string GetDisplayField(Type type)
        {
            return type.GetCustomAttribute<ContentTypeDisplayFieldAttribute>()?.FieldName ??
                   type.GetProperties().FirstOrDefault()?.Name;
        }

        private IList<Type> FindChildren(Type baseType)
        {
            var children = new List<Type>();
            var subclasses = _availableContentTypes.Where(t => t.IsSubclassOf(baseType)).ToList();
            foreach (var subclass in subclasses)
            {
                if (subclass.BaseType.GUID == baseType.GUID)
                {
                    children.Add(subclass);
                }
                else
                {
                    var contentTypeBaseClass = GetInheritancePredecessors(subclass)
                        .FirstOrDefault(t => GetContentTypeAttribute(t) != null);

                    if (contentTypeBaseClass != null && contentTypeBaseClass.GUID == baseType.GUID)
                    {
                        children.Add(subclass);
                    }
                }
            }

            return children;
        }
    }
}