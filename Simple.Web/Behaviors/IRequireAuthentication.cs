namespace Simple.Web.Behaviors
{
    [RequestBehavior(typeof(Implementations.CheckAuthentication), Priority = Priority.Highest)]
    public interface IRequireAuthentication
    {
        IUser CurrentUser { set; }
    }
}