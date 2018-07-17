using System.Linq;
using System.Reflection;
using Forte.ContentfulSchema.Conventions;
using Forte.ContentfulSchema.Discovery;
using Microsoft.Extensions.DependencyInjection;

namespace Forte.ContentfulSchema
{
    public static class ServiceCollectionExtensions
    {
        public static void AddContentSchemaFromAssemblyOf<TApp>(this IServiceCollection services)
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
            
            services.AddSingleton(schema);
        }
    }
}