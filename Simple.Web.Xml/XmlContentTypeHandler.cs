using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Xml
{
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;
    using ContentTypeHandling;
    using Links;

    public class XmlContentTypeHandler : IContentTypeHandler
    {
        public object Read(Stream inputStream, Type inputType)
        {
            var dataContractSerializer = new DataContractSerializer(inputType);
            return dataContractSerializer.ReadObject(inputStream);
        }

        public void Write(IContent content, Stream outputStream)
        {
            if (content.Model != null)
            {
                var enumerable = content.Model as IEnumerable<object>;
                if (enumerable != null)
                {
                    XElement collection = null;
                    foreach (var element in ProcessList(enumerable))
                    {
                        if (collection == null)
                        {
                            var plural = PluralizationService.CreateService(CultureInfo.CurrentCulture);
                            collection = new XElement(plural.Pluralize(element.Name.LocalName));
                        }
                        collection.Add(element);
                    }
                    if (collection != null)
                    {
                        WriteXml(outputStream, collection);
                    }
                }
                else
                {
                    var links = content.Links.ToList();
                    if (links.Count == 0)
                    {
                        new DataContractSerializer(content.Model.GetType()).WriteObject(outputStream, content.Model);
                    }
                    else
                    {
                        WriteWithLinks(content, outputStream, links);
                    }
                }
            }
        }

        private static IEnumerable<XElement> ProcessList(IEnumerable<object> source)
        {
            bool skipLinkCheck = false;
            foreach (var o in source)
            {
                if (!skipLinkCheck)
                {
                    var links = LinkHelper.GetLinksForModel(o).ToList();
                    if (links.Count == 0)
                    {
                        skipLinkCheck = true;
                    }
                    else
                    {
                        yield return CreateElementWithLinks(o, links);
                        continue;
                    }
                }

                yield return CreateElement(o);
            }
        }

        private static void WriteWithLinks(IContent content, Stream outputStream, IEnumerable<Link> links)
        {
            var xml = CreateElementWithLinks(content.Model, links);

            WriteXml(outputStream, xml);
        }

        private static void WriteXml(Stream outputStream, XElement xml)
        {
            using (var writer = XmlWriter.Create(outputStream))
            {
                xml.WriteTo(writer);
            }
        }

        private static XElement CreateElement(object model)
        {
            var stringBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(stringBuilder);

            new DataContractSerializer(model.GetType()).WriteObject(xmlWriter, model);
            xmlWriter.Flush();

            return XElement.Parse(stringBuilder.ToString());
        }
        
        private static XElement CreateElementWithLinks(object model, IEnumerable<Link> links)
        {
            var xml = CreateElement(model);

            foreach (var link in links)
            {
                var linkElement = new XElement("link");
                linkElement.SetAttributeValue("title", link.Title);
                linkElement.SetAttributeValue("href", link.Href);
                linkElement.SetAttributeValue("rel", link.Rel);
                linkElement.SetAttributeValue("type", EnsureXml(link.Type));
                xml.Add(linkElement);
            }
            return xml;
        }

        private static string EnsureXml(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return ContentType.Xml;
            if (type.EndsWith("xml")) return type;
            return type + "+xml";
        }
    }
}
