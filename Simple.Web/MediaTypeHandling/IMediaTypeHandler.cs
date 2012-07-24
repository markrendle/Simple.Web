namespace Simple.Web.MediaTypeHandling
{
    using System;
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
        /// <param name="inputStream">The input stream.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <returns>A model constructed from the content in the input stream.</returns>
        object Read(Stream inputStream, Type inputType);

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="outputStream">The output stream.</param>
        Task Write(IContent content, Stream outputStream);
    }
}