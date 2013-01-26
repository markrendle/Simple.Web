namespace Simple.Web
{
    using System.Collections.Generic;
    using Helpers;

    /// <summary>
    /// Provides information about the environment for the application.
    /// </summary>
    public interface IWebEnvironment
    {
        /// <summary>
        /// Gets the root folder of the application in the host.
        /// </summary>
        string AppRoot { get; }

        /// <summary>
        /// Gets the path utility.
        /// </summary>
        IPathUtility PathUtility { get; }

        /// <summary>
        /// Gets the file utility.
        /// </summary>
        IFileUtility FileUtility { get; }

        /// <summary>
        /// Gets the content type from a file extension.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="acceptedTypes">The accepted types.</param>
        /// <returns>The acceptable type for the file.</returns>
        string GetMediaTypeFromFileExtension(string file, IList<string> acceptedTypes);
    }
}