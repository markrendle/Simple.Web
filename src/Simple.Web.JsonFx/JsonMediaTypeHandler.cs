using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.JsonFx
{
    using System.IO;
    using System.Threading.Tasks;
    using Helpers;
    using Links;
    using MediaTypeHandling;
    using global::JsonFx.Model.Filters;
    using global::JsonFx.Json;
    using global::JsonFx.Json.Resolvers;
    using global::JsonFx.Serialization;
    using global::JsonFx.Serialization.Resolvers;
    using global::JsonFx.Xml.Resolvers;

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class JsonMediaTypeHandler : IMediaTypeHandler
    {
        public Task<T> Read<T>(Stream inputStream)
        {
            return Task<T>.Factory.StartNew(() =>
                {
                    var resolver = CreateResolverStrategy();

                    // pass the combined resolver strategy into the settings object
                    var reader = new JsonReader(new DataReaderSettings(resolver));

                    using (var streamReader = new StreamReader(inputStream))
                    {
                        return reader.Read<T>(streamReader);
                    }
                });
        }

        private static CombinedResolverStrategy CreateResolverStrategy()
        {
            return new CombinedResolverStrategy(
                new JsonResolverStrategy(), // simple JSON attributes
                new DataContractResolverStrategy(), // DataContract attributes
                new XmlResolverStrategy(), // XmlSerializer attributes
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.PascalCase), // DotNetStyle
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.CamelCase), // jsonStyle
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.Lowercase, "-"), // xml-style
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.Uppercase, "_")); // CONST_STYLE
        }

        public Task Write<T>(IContent content, Stream outputStream)
        {
            try
            {
                if (content.Model != null)
                {
                    object output;

                    var enumerable = content.Model as IEnumerable<object>;
                    if (enumerable != null)
                    {
                        output = ProcessList(enumerable.ToList());
                    }
                    else
                    {
                        output = ProcessContent(content);
                    }
                    byte[] buffer;
                    using (var writer = new StringWriter())
                    {
                        var dataWriterSettings = new DataWriterSettings(new MonoCompatResolverStrategy(),
                                                                        new Iso8601DateFilter());

                        new JsonWriter(dataWriterSettings).Write(output, writer);
                        buffer = Encoding.UTF8.GetBytes(writer.ToString());
                    }
                    return outputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                return TaskHelper.Exception(ex);
            }

            return TaskHelper.Completed();
        }

        private static object ProcessContent(IContent content)
        {
            var links = content.Links.Select(EnsureJson).ToList();
            if (links.Count == 0)
            {
                return content.Model;
            }
            var dictionary = content.Model.ToDictionary();
            dictionary.Add("links", links);
            return dictionary;
        }

        private static IEnumerable<object> ProcessList(IEnumerable<object> source)
        {
            bool skipLinkCheck = false;
            foreach (var o in source)
            {
                if (!skipLinkCheck)
                {
                    var links = LinkHelper.GetLinksForModel(o).Select(EnsureJson).ToList();
                    if (links.Count == 0)
                    {
                        skipLinkCheck = true;
                    }
                    else
                    {
                        var dictionary = o.ToDictionary();
                        dictionary.Add("links", links);
                        yield return dictionary;
                        continue;
                    }
                }

                yield return o;
            }
        }

        private static Link EnsureJson(Link source)
        {
            if (!string.IsNullOrWhiteSpace(source.Type))
            {
                if (source.Type.EndsWith("json"))
                {
                    return source;
                }
                return new Link(source.GetHandlerType(), source.Href, source.Rel, source.Type + "+json", source.Title);
            }
            return new Link(source.GetHandlerType(), source.Href, source.Rel, MediaType.Json, source.Title);
        }
    }
}