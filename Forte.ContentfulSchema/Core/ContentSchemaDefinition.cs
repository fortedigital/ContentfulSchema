using System;
using System.Collections.Generic;
using System.Linq;

namespace Forte.ContentfulSchema.Core
{
    public class ContentSchemaDefinition
    {
        public readonly IReadOnlyDictionary<Type, ContentTypeDefinition> ContentTypeLookup;

        public IEnumerable<ContentTypeDefinition> ContentTypeDefinitions => this.ContentTypeLookup.Values;
        public IReadOnlyDictionary<string, Type> ClrTypeLookup { get; }

        public ContentSchemaDefinition(IReadOnlyDictionary<Type, ContentTypeDefinition> contentTypeLookup)
        {
            this.ContentTypeLookup = contentTypeLookup;
            this.ClrTypeLookup = contentTypeLookup.ToDictionary(
                kvp => kvp.Value.ContentType.SystemProperties.Id, 
                kvp => kvp.Key);
        }
    }
}