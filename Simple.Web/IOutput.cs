namespace Simple.Web
{
    using System.IO;

    /// <summary>
    /// Adds output functionality to an handler.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IOutput<out TOutput>
    {
        /// <summary>
        /// Gets the output.
        /// </summary>
        TOutput Output { get; }
    }

    /// <summary>
    /// Adds functionality for handlers which return raw streams.
    /// </summary>
    public interface IOutputStream : IOutput<Stream>
    {
        /// <summary>
        /// Gets the text to use as the <c>Content-Type</c> header.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the text to use in the <c>Content-Disposition header</c>.
        /// </summary>
        /// <remarks>Return <c>null</c> to omit the <c>Content-Disposition</c> header.
        /// Use a string like &quot;attachment; filename=myfile.txt&quot; to force the browser to download
        /// the content as a file instead of displaying it.</remarks>
        string ContentDisposition { get; }
    }
}