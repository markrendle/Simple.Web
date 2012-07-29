namespace Simple.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// Extension method class for <see cref="NameValueCollection"/>.
    /// </summary>
    public static class NameValueCollectionEx
    {
        /// <summary>
        /// Converts a <see cref="NameValueCollection"/> to an ILookup.
        /// </summary>
        /// <param name="nameValueCollection">The name value collection.</param>
        /// <returns>An <see cref="ILookup{TKey,TValue}"/> representation of the <see cref="NameValueCollection"/>.</returns>
        public static ILookup<string,string> ToLookup(this NameValueCollection nameValueCollection)
        {
            if (nameValueCollection == null) throw new ArgumentNullException("nameValueCollection");

            return nameValueCollection.AllKeys.SelectMany(
                k => nameValueCollection.GetValues(k).EmptyIfNull().Select(v => new KeyValuePair<string, string>(k, v)))
                .ToLookup(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}