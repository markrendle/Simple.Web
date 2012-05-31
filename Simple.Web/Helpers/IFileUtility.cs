namespace Simple.Web.Helpers
{
    /// <summary>
    /// Provides methods for working with files in an application server.
    /// </summary>
    public interface IFileUtility
    {
        /// <summary>
        /// Checks to see whether a path exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if the path exists; otherwise <c>false</c>.</returns>
        bool Exists(string path);
    }
}