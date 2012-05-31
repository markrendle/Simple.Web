namespace Simple.Web.Behaviors
{
    using System.IO;

    /// <summary>
    /// Adds functionality for handlers which return raw streams.
    /// </summary>
    [OutputBehavior(typeof(Implementations.WriteStreamResponse), Priority = Priority.Highest)]
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