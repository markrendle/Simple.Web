namespace Simple.Web.Behaviors
{
    using Simple.Web.Behaviors.Implementations;
    using Simple.Web.Http;

    /// <summary>
    /// Indicates that a handler exposes caching information.
    /// </summary>
    [ResponseBehavior(typeof(SetCacheOptions))]
    public interface ICacheability
    {
        /// <summary>
        /// Gets the cache options.
        /// </summary>
        CacheOptions CacheOptions { get; }
    }
}