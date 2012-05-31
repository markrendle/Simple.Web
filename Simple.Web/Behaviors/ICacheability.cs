namespace Simple.Web.Behaviors
{
    using Http;

    [ResponseBehavior(typeof(Implementations.SetCache))]
    public interface ICacheability
    {
        CacheOptions CacheOptions { get; }
    }
}