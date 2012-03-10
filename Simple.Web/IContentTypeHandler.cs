namespace Simple.Web
{
    using System.IO;

    public interface IContentTypeHandler
    {
        object Read(StreamReader streamReader);
        void Write(object obj, StreamWriter streamWriter);
    }
}