using System;
using System.Collections.Generic;

namespace Forte.ContentfulSchema.Core
{
    public class SchemaDefinition
    {
        public readonly IReadOnlyDictionary<Type, ContentTypeDefinition> ContentTypeDefinitions;

        public SchemaDefinition(IReadOnlyDictionary<Type, ContentTypeDefinition> contentTypeDefinitions)
        {
            this.ContentTypeDefinitions = contentTypeDefinitions;
        }
    }
}