namespace Simple.Web.Helpers
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Async operations
    /// </summary>
    public static class AsyncExtensions
    {
        /// <summary>
        /// Write a byte array to a Stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="bytes">The bytes to write to the stream.</param>
        /// <param name="offset">The index within the byte array from which to write.</param>
        /// <param name="length">The number of bytes to write to the stream.</param>
        /// <returns>A Task which will complete when all bytes have been written to the Stream.</returns>
        public static Task WriteAsync(this Stream stream, byte[] bytes, int offset, int length)
        {
            return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, bytes, offset, length, null);
        }
    }
}