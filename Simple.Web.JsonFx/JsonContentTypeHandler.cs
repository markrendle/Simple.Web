using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.JsonFx
{
    using System.IO;
    using global::JsonFx.Json;

    [ContentTypes("application/json")]
    public class JsonContentTypeHandler : IContentTypeHandler
    {
        public object Read(StreamReader streamReader)
        {
            return new JsonReader().Read(streamReader);
        }

        public void Write(object obj, StreamWriter streamWriter)
        {
            new JsonWriter().Write(obj, streamWriter);
        }
    }
}
