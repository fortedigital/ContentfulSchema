using System.Linq;
using Forte.ContentfulSchema.Discovery;

namespace Forte.ContentfulSchema.Tests.Discovery
{
    internal static class ContentTreeTestExtensions
    {
        public static IContentNode GetRootOfType<TRoot>(this IContentTree tree)
        {
            return tree.Roots.Single(r => r.ClrType == typeof(TRoot));
        }
    }
}