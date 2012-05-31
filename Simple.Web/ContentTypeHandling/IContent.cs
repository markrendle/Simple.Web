namespace Simple.Web.ContentTypeHandling
{
    using System.Collections.Generic;
    using Links;

    /// <summary>
    /// Represents content to be returned to the client.
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Gets the handler which generated the model.
        /// </summary>
        object Handler { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        object Model { get; }

        /// <summary>
        /// Gets the variables from the handler.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Variables { get; }

        /// <summary>
        /// Gets the links which are valid for the model type, based on the <see cref="LinksFromAttribute"/> on handlers.
        /// </summary>
        IEnumerable<Link> Links { get; } 
    }
}