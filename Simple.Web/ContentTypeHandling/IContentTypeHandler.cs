namespace Simple.Web.ContentTypeHandling
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
        object Handler { get; }
        object Model { get; }
        IEnumerable<KeyValuePair<string, object>> Variables { get; }
        string ViewPath { get; }
    }
}