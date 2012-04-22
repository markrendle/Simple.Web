using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.JsonFx
{
    using System.IO;
    using global::JsonFx.Json;
    using global::JsonFx.Json.Resolvers;
    using global::JsonFx.Serialization;
    using global::JsonFx.Serialization.Resolvers;
    using global::JsonFx.Xml.Resolvers;

    [ContentTypes(ContentType.Json)]
    public class JsonContentTypeHandler : IContentTypeHandler
    {
        public object Read(StreamReader streamReader, Type inputType)
        {
            var resolver = new CombinedResolverStrategy(
                new JsonResolverStrategy(),                                                             // simple JSON attributes
                new DataContractResolverStrategy(),                                                     // DataContract attributes
                new XmlResolverStrategy(),                                                              // XmlSerializer attributes
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.PascalCase),       // DotNetStyle
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.CamelCase),        // jsonStyle
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.Lowercase, "-"),   // xml-style
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.Uppercase, "_"));  // CONST_STYLE

            // pass the combined resolver strategy into the settings object
            var reader = new JsonReader(new DataReaderSettings(resolver));

            return reader.Read(streamReader, inputType);
        }

        public void Write(IContent content, TextWriter textWriter)
        {
            if (content.Model != null)
                new JsonWriter().Write(content.Model, textWriter);
        }
    }
}
