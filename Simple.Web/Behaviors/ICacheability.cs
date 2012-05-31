namespace Simple.Web.Behaviors
{
    using Http;

    /// <summary>
    /// Indicates that a handler exposes caching information.
    /// </summary>
    [ResponseBehavior(typeof(Implementations.SetCache))]
    public interface ICacheability
    {
        /// <summary>
        /// Gets the cache options.
        /// </summary>
        CacheOptions CacheOptions { get; }
    }
}