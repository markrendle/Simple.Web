namespace Simple.Web
{
    using System.IO;
    using System.Reflection;

    public sealed class WebEnvironment : IWebEnvironment
    {
        private static readonly string BinBasedAppRoot =
            Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetPath()));

        private readonly IPathUtility _pathUtility = new PathUtility();
        private readonly IFileUtility _fileUtility = new FileUtility();

        public string AppRoot
        {
            get { return BinBasedAppRoot; }
        }

        public IPathUtility PathUtility
        {
            get { return _pathUtility; }
        }

        public IFileUtility FileUtility
        {
            get { return _fileUtility; }
        }
    }
}