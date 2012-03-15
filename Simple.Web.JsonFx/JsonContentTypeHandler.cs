using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.JsonFx
{
    using System.IO;
    using global::JsonFx.Json;

    [ContentTypes(ContentType.Json)]
    public class JsonContentTypeHandler : IContentTypeHandler
    {
        public object Read(StreamReader streamReader, Type inputType)
        {
            return new JsonReader().Read(streamReader, inputType);
        }

        public void Write(IEndpoint endpoint, TextWriter textWriter)
        {
            new JsonWriter().Write(endpoint.Output, textWriter);
        }
    }
}
