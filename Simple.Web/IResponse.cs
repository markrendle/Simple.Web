namespace Simple.Web
{
    using System.IO;

    public interface IResponse
    {
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        TextWriter Output { get; }
        Stream OutputStream { get; }
        string ContentType { get; set; }
        void Close();
        void Flush();
        void Write(string s);
        void Write(object obj);
        void Write(char[] buffer, int index, int count);
        void Write(char ch);
        void TransmitFile(string file);
    }
}