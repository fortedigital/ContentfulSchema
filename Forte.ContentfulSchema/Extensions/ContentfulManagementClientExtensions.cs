using System.Reflection;
using System.Threading.Tasks;
using Contentful.Core;
using Forte.ContentfulSchema.Core;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema.Extensions
{
    public static class ContentfulManagementClientExtensions
    {
        public static async Task SyncContentTypes<TApp>(this IContentfulManagementClient client)
        {
            //var schemaGenerator = new SchemaGenerator();
            var contentSchemaGenerator = new ContentSchemaGenerator(
                new ContentFieldTypeProvider(), 
                new ContentEditorControlProvider());
            var schemaMerger = new SchemaManager(client);

            //var inferedContentTypes = schemaGenerator.GenerateSchema(typeof(TApp).GetTypeInfo().Assembly.GetTypes());
            var contentTreeBuilder = new ContentTreeBuilder(typeof(TApp).GetTypeInfo().Assembly.GetTypes());
            var inferedTypes = contentSchemaGenerator.GenerateContentSchema(contentTreeBuilder.DiscoverContentStructure());

            //var existingContentTypes = await client.GetContentTypesAsync();
            //await schemaMerger.MergeSchema(inferedContentTypes, existingContentTypes);

            await schemaMerger.UpdateSchema(inferedTypes);
        }
    }
}