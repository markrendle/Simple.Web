namespace Simple.Web
{
    using System.IO;

    public interface IPostedFile
    {
        /// <summary>
        /// Saves the contents of an uploaded file.
        /// </summary>
        /// <param name="filename">The name of the saved file. </param><exception cref="T:System.Web.HttpException">The <see cref="P:System.Web.Configuration.HttpRuntimeSection.RequireRootedSaveAsPath"/> property of the <see cref="T:System.Web.Configuration.HttpRuntimeSection"/> object is set to true, but <paramref name="filename"/> is not an absolute path.</exception>
        void SaveAs(string filename);

        /// <summary>
        /// Gets the fully qualified name of the file on the client.
        /// </summary>
        /// <returns>
        /// The name of the client's file, including the directory path.
        /// </returns>
        string FileName { get; }

        /// <summary>
        /// Gets the MIME content type of a file sent by a client.
        /// </summary>
        /// <returns>
        /// The MIME content type of the uploaded file.
        /// </returns>
        string ContentType { get; }

        /// <summary>
        /// Gets the size of an uploaded file, in bytes.
        /// </summary>
        /// <returns>
        /// The file length, in bytes.
        /// </returns>
        int ContentLength { get; }

        /// <summary>
        /// Gets a <see cref="T:System.IO.Stream"/> object that points to an uploaded file to prepare for reading the contents of the file.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.IO.Stream"/> pointing to a file.
        /// </returns>
        Stream InputStream { get; }
    }
}