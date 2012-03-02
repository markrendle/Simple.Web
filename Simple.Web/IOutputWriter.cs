namespace Simple.Web
{
    using System.IO;

    internal interface IOutputWriter
    {
        string ContentType { get; }
        void Write(Stream outputStream, object output);
    }
}