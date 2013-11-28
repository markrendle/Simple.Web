namespace Simple.Web.JsonNet
{
    using System.Collections.Generic;
    using Links;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Interface for types that format <see cref="Link"/> collections for JSON.NET
    /// </summary>
    /// <remarks>
    /// The implementing types do not write the actual JSON text, rather, they create a JSON.NET <see cref="JToken"/>
    /// allowing them to specify the structure or layout of the object.
    /// </remarks>
    public interface IJsonLinksFormatter
    {
        /// <summary>
        /// Formats the links for a JSON item.
        /// </summary>
        /// <param name="container">The object to which the links will be added</param>
        /// <param name="links">The links. Guaranteed to be non-empty.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        void FormatLinks(JContainer container, IEnumerable<Link> links, JsonSerializer serializer);
    }
}