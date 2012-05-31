namespace Simple.Web.Helpers
{
    /// <summary>
    /// Provides methods for working with virtual paths in an application server.
    /// </summary>
    public interface IPathUtility
    {
        /// <summary>
        /// Maps a virtual path to its internal file-system representation.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        string MapPath(string virtualPath);
    }
}