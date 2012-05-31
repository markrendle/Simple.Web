namespace Simple.Web.Http
{
    public interface IContext
    {
        IRequest Request { get; }
        IResponse Response { get; }
        IUser User { get; }
    }
}