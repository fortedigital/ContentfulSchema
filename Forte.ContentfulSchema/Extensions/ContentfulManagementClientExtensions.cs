using System.Reflection;
using System.Threading.Tasks;
using Contentful.Core;
using Forte.ContentfulSchema.Core;

namespace Forte.ContentfulSchema.Extensions
{
    public static class ContentfulManagementClientExtensions
    {
        public static async Task SyncContentTypes<TApp>(this IContentfulManagementClient client)
        {
            var schemaGenerator = new SchemaGenerator();
            var schemaMerger = new SchemaMerger(client);

            var inferedContentTypes = schemaGenerator.GenerateSchema(typeof(TApp).GetTypeInfo().Assembly.GetTypes());

            var existingContentTypes = await client.GetContentTypes();
            await schemaMerger.MergeSchema(inferedContentTypes, existingContentTypes);
        }
    }
}