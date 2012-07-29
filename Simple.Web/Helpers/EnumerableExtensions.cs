namespace Simple.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}