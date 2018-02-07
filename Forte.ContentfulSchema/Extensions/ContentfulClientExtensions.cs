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
        /// <summary>
        /// Get content of specified type using ContentType attribute of a type T
        /// </summary>
        /// <param name="client">Contentful client</param>
        /// <param name="queryBuilder">QueryBuilder to filter entries</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Entiries for specified content type</returns>
        public static Task<ContentfulCollection<T>> GetContentForTypeAsync<T>(this IContentfulClient client, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contentTypeDefinition = typeof(T).GetTypeInfo()
                .GetCustomAttributes<ContentTypeAttribute>().Single();

            return client.GetEntriesByType<T>(contentTypeDefinition.ContentTypeId, queryBuilder, cancellationToken);
        }

        /// <summary>
        /// Get Entries of specified type using ContentType attribute of a type T
        /// </summary>
        /// <param name="client"></param>
        /// <param name="queryBuilder"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<ContentfulCollection<Entry<T>>> GetEntriesByTypeAsync<T>(this IContentfulClient client, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contentTypeDefinition = typeof(T).GetTypeInfo().GetCustomAttributes<ContentTypeAttribute>().Single();

            queryBuilder = queryBuilder ?? new QueryBuilder<T>();
            queryBuilder.ContentTypeIs(contentTypeDefinition.ContentTypeId);

            return client.GetEntries<Entry<T>>(queryBuilder.Build(), cancellationToken);
        }
    }
}