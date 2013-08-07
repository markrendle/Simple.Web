namespace Simple.Web.Xml
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Inflector;
    using Helpers;
    using Links;
    using MediaTypeHandling;

    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class XmlMediaTypeHandler : IMediaTypeHandler
    {
        public Task<T> Read<T>(Stream inputStream)
        {
            return Task<T>.Factory.StartNew(() =>
                {
                    var dataContractSerializer = new DataContractSerializer(typeof (T));
                    return (T) dataContractSerializer.ReadObject(inputStream);
                });
        }

        public Task Write<T>(IContent content, Stream outputStream)
        {
            if (content.Model != null)
            {
                var serializer = new DataContractSerializer(typeof(T));

                XElement xml = null;

                var enumerable = content.Model as IEnumerable<T>;
                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        var itemLinks = LinkHelper.GetLinksForModel(item);
                        var element = ItemToXml(item, itemLinks, serializer);
                        if (xml == null)
                        {
                            xml = new XElement(element.Name.LocalName.Pluralize());
                        }
                        xml.Add(element);
                    }
                    // todo: any links at root level?
                }
                else
                {
                    var links = content.Links;
                    xml = ItemToXml((T) content.Model, links, serializer);
                }
                if (xml != null)
                {
                    xml.Save(outputStream);
                }
            }
            return TaskHelper.Completed();
        }

        private static XElement ItemToXml<T>(T item, IEnumerable<Link> links, DataContractSerializer serializer)
        {
            var stringBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(stringBuilder);
            serializer.WriteObject(xmlWriter, item);
            xmlWriter.Flush();
            var xml = XElement.Parse(stringBuilder.ToString());
            foreach (var link in links)
            {
                xml.Add(link.ToXml());
            }
            return xml;
        }
    }
}