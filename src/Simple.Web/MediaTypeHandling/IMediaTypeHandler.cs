namespace Simple.Web.MediaTypeHandling
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a handler for a specific content type.
    /// </summary>
    public interface IMediaTypeHandler
    {
        /// <summary>
        /// Reads content from the specified input stream.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the stream.</typeparam>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>A model constructed from the content in the input stream.</returns>
        Task<T> Read<T>(Stream inputStream);

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <typeparam name="T">The type of the object to write to the stream.</typeparam>
        /// <param name="content">The content.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <returns>A <see cref="Task"/> that completes when the output has been written.</returns>
        Task Write<T>(IContent content, Stream outputStream);
    }
}