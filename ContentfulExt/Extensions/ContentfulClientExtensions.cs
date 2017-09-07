using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core;
using ContentfulExt.Attributes;
using System.Reflection;
using System.Linq;
using System.Threading;
using Contentful.Core.Search;

namespace ContentfulExt.Extensions
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