namespace Simple.Web
{
    using System.IO;

    internal interface IResponse
    {
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        Stream OutputStream { get; }
        string ContentType { get; set; }
        void Close();
        void Flush();
    }
}