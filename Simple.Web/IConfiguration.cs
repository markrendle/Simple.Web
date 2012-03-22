namespace Simple.Web
{
    using System.Collections.Generic;

    public interface IConfiguration
    {
        ISet<string> PublicFolders { get; }
    }
}