namespace Simple.Web
{
    using System.Collections.Generic;

    public sealed class Configuration : IConfiguration
    {
        private readonly HashSet<string> _publicFolders = new HashSet<string>();

        public ISet<string> PublicFolders { get { return _publicFolders; } } 
    }
}