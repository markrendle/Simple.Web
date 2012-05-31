namespace Simple.Web.Behaviors
{
    [RequestBehavior(typeof(Implementations.SetInputETag))]
    [ResponseBehavior(typeof(Implementations.SetOutputETag))]
    public interface IETag
    {
        string InputETag { set; }
        string OutputETag { get; }
    }
}