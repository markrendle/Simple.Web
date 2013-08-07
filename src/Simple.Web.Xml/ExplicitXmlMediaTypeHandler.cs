namespace Simple.Web.Xml
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Inflector;
    using Helpers;
    using Links;
    using MediaTypeHandling;

    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class ExplicitXmlMediaTypeHandler : IMediaTypeHandler
    {
        public Task<T> Read<T>(Stream inputStream)
        {
            return Task<T>.Factory.StartNew(() =>
                {
                    IMediaConverter<T, XElement> converter;
                    using (var container = SimpleWeb.Configuration.Container.BeginScope())
                    {
                        converter = container.Get<IMediaConverter<T, XElement>>();
                    }
                    var xml = XElement.Load(inputStream);
                    return converter.FromWireFormat(xml);
                });
        }

        public Task Write<T>(IContent content, Stream outputStream)
        {
            if (content.Model != null)
            {
                IMediaConverter<T, XElement> converter;
                using (var container = SimpleWeb.Configuration.Container.BeginScope())
                {
                    converter = container.Get<IMediaConverter<T, XElement>>();
                }

                XElement xml = null;

                // handle enumerable
                var enumerable = content.Model as IEnumerable<T>;
                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        var itemLinks = LinkHelper.GetLinksForModel(item);
                        var element = ItemToXml(item, itemLinks, converter);
                        if (xml == null)
                        {
                            // todo: should this be explicit too?
                            xml = new XElement(element.Name.LocalName.Pluralize());
                        }
                        xml.Add(element);
                    }
                    // todo: any links at root level?
                }
                else
                {
                    var links = content.Links.ToList();
                    xml = ItemToXml((T) content.Model, links, converter);
                }
                if (xml != null)
                {
                    xml.Save(outputStream);
                }
            }
            return TaskHelper.Completed();
        }

        private static XElement ItemToXml<T>(T item, IEnumerable<Link> links, IMediaConverter<T, XElement> converter)
        {
            var xml = converter.ToWireFormat(item);
            foreach (var link in links)
            {
                xml.Add(link.ToXml());
            }
            return xml;
        }
    }
}