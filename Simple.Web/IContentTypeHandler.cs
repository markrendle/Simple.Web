namespace Simple.Web
{
    using System;
    using System.IO;

    public interface IContentTypeHandler
    {
        object Read(StreamReader streamReader, Type inputType);
        void Write(object obj, StreamWriter streamWriter);
    }
}