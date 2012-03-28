namespace Simple.Web
{
    using System.Collections.Generic;
    using DependencyInjection;

    public sealed class Configuration : IConfiguration
    {
        private readonly HashSet<string> _publicFolders = new HashSet<string>();

        public ISet<string> PublicFolders { get { return _publicFolders; } }

        private ISimpleContainer _container = new DefaultSimpleContainer();
        public ISimpleContainer Container
        {
            get { return _container; }
            set { _container = value ?? new DefaultSimpleContainer(); }
        }
    }
}