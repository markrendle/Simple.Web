namespace Simple.Web
{
    public interface IContext
    {
        IRequest Request { get; }
        IResponse Response { get; }
    }
}