using System;
using System.Xml.Linq;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
    public class LinkToXml : XmlConverter<Link>
    {
        public static readonly LinkToXml Instance = new LinkToXml();

        public override Link FromXml(XElement wireFormat)
        {
            // come back to this when necessary
            throw new NotSupportedException();
        }

        public override XElement ToXml(Link value)
        {
            var linkElement = new XElement("link");
            linkElement.SetAttributeValue("title", value.Title);
            linkElement.SetAttributeValue("href", value.Href);
            linkElement.SetAttributeValue("rel", value.Rel);
            linkElement.SetAttributeValue("type", EnsureTypeIsXml(value.Type));
            return linkElement;
        }

        private static string EnsureTypeIsXml(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return MediaType.Xml;
            if (type.EndsWith("xml")) return type;
            return type + "+xml";
        }
    }

    public static class LinkToXmlExtensions
    {
        public static Link ToLink(this XElement element)
        {
            return LinkToXml.Instance.FromXml(element);
        }

        public static XElement ToXml(this Link value)
        {
            return LinkToXml.Instance.ToXml(value);
        }
    }
}