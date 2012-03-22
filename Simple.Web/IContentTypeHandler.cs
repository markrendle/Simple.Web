namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IContentTypeHandler
    {
        object Read(StreamReader streamReader, Type inputType);
        void Write(IContent content, TextWriter textWriter);
    }

    public interface IContent
    {
        object Model { get; }
        IEnumerable<KeyValuePair<string, object>> Variables { get; }
        string ViewPath { get; }
    }
}