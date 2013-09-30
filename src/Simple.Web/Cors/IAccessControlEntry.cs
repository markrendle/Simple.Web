namespace Simple.Web.Cors
{
    public interface IAccessControlEntry
    {
        string AllowHeaders { get; }

        bool? Credentials { get; }

        string ExposeHeaders { get; }

        long? MaxAge { get; }

        string Methods { get; }

        string Origin { get; }
    }
}