using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Forte.ContentfulSchema.Attributes;

namespace Forte.ContentfulSchema.Extensions
{
    public static class ContentfulClientExtensions
    {
        public static Task<ContentfulCollection<T>> GetContentByTypeAsync<T>(this IContentfulClient client, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contentTypeDefinition = typeof(T).GetTypeInfo()
                .GetCustomAttributes<ContentTypeAttribute>().Single();

            return client.GetEntriesByTypeAsync<T>(contentTypeDefinition.ContentTypeId, queryBuilder, cancellationToken);
        }

        public static Task<ContentfulCollection<Entry<T>>> GetEntriesByTypeAsync<T>(this IContentfulClient client, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contentTypeDefinition = typeof(T).GetTypeInfo().GetCustomAttributes<ContentTypeAttribute>().Single();

            queryBuilder = queryBuilder ?? new QueryBuilder<T>();
            queryBuilder.ContentTypeIs(contentTypeDefinition.ContentTypeId);

            return client.GetEntriesAsync<Entry<T>>(queryBuilder.Build(), cancellationToken);
        }
    }
}