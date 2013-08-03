using System.Xml.Linq;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.ExplicitXml
{
	public class LinkToXml : IMediaConverter<Link,XElement>
	{
		public static readonly LinkToXml Instance = new LinkToXml();

		private static string EnsureTypeIsXml(string type)
		{
			if (string.IsNullOrWhiteSpace(type)) return MediaType.Xml;
			if (type.EndsWith("xml")) return type;
			return type + "+xml";
		}

	    public Link FromWireFormat(XElement wireFormat)
	    {
            // come back to this when necessary
            throw new System.NotImplementedException();
        }

	    public Link FromWireFormat(XElement wireFormat, Link loadThis)
	    {
            // come back to this when necessary
            throw new System.NotImplementedException();
        }

	    public XElement ToWireFormat(Link value)
	    {
            var linkElement = new XElement("link");
            linkElement.SetAttributeValue("title", value.Title);
            linkElement.SetAttributeValue("href", value.Href);
            linkElement.SetAttributeValue("rel", value.Rel);
            linkElement.SetAttributeValue("type", EnsureTypeIsXml(value.Type));
            return linkElement;
        }
	}

	public static class LinkToXmlExtensions
	{
		public static Link ToLink(this XElement element)
		{
			return LinkToXml.Instance.FromWireFormat(element);
		}

		public static XElement ToXml(this Link value)
		{
			return LinkToXml.Instance.ToWireFormat(value);
		}
	}
}