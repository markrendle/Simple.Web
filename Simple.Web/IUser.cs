namespace Simple.Web
{
    public interface IUser
    {
        string Name { get; }
        bool IsAuthenticated { get; }
    }
}