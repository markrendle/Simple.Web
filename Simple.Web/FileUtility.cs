namespace Simple.Web
{
    using System.IO;

    internal sealed class FileUtility : IFileUtility
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}