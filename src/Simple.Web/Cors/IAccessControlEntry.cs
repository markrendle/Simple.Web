namespace Simple.Web.Cors
{
    public interface IAccessControlEntry
    {
        string Origin { get; }
        bool? Credentials { get; }
        string Methods { get; }
        string AllowHeaders { get; }
        string ExposeHeaders { get; }
        long? MaxAge { get; }
    }
}