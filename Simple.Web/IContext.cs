namespace Simple.Web
{
    internal interface IContext
    {
        IRequest Request { get; }
        IResponse Response { get; }
    }
}