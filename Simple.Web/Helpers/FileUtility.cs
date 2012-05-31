namespace Simple.Web.Helpers
{
    using System.IO;

    internal sealed class FileUtility : IFileUtility
    {
        /// <summary>
        /// Checks to see whether a path exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if the path exists; otherwise <c>false</c>.
        /// </returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}