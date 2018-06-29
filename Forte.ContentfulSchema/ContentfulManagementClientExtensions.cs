using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Contentful.Core;
using Forte.ContentfulSchema.Conventions;
using Forte.ContentfulSchema.Core;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema
{
    public static class ContentfulManagementClientExtensions
    {
        public static async Task<SchemaDefinition> UpdateSchemaAsync<TApp>(this IContentfulManagementClient client)
        {
            var namingConventions = new DefaultNamingConventions();
            var fieldTypeConvention = ContentTypeFieldTypeConvention.Default;
            var validationProviders = new[]
            {
                new LinkContentTypeValidatorProvider()
            };

            var discoveryService = new SchemaDiscoveryService(
                namingConventions,
                namingConventions, 
                DefaultPropertyIgnoreConvention.Default, 
                fieldTypeConvention, DefaultFieldControlConvention.Default, validationProviders);
            
            var schema = discoveryService.DiscoverSchema(typeof(TApp).GetTypeInfo().Assembly.GetTypes());

            await client.UpdateSchemaAsync(schema);

            return schema;
        }

        public static async Task UpdateSchemaAsync(this IContentfulManagementClient client, SchemaDefinition schema)
        {
            var schemaSyncService = new SchemaSynchronizationService(client);
            await schemaSyncService.UpdateSchema(schema.ContentTypeDefinitions.Select(kvp => kvp.Value));
        }
    }
}