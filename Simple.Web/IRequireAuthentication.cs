namespace Simple.Web
{
    public interface IRequireAuthentication
    {
        IUser CurrentUser { get; set; }
    }
}