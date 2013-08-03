using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Inflector;
using Simple.Web.ExplicitXml;
using Simple.Web.Helpers;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
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
            if (content.Model == null)
            {
                return TaskHelper.Completed();
            }
            return Task.Factory.StartNew(() =>
                {
                    XElement xml;

                    // handle enumerable
                    var enumerable = content.Model as IEnumerable<T>;
                    if (enumerable != null)
                    {
                        xml = new XElement("List");
                        foreach (var item in enumerable)
                        {
                            T typedItem = (T) item;
                            var itemLinks = LinkHelper.GetLinksForModel(item).ToList();
                            var element = ItemToXml(typedItem, itemLinks);
                            xml.Add(element);
                        }
                        // should this be explicit too?
                        xml.Name = xml.Elements().First().Name.LocalName.Pluralize();
                    }
                    else
                    {
                        T typedModel = (T) content.Model;
                        var links = content.Links.ToList();
                        xml = ItemToXml(typedModel, links);
                    }
                    xml.Save(outputStream);
                });
        }

        private static XElement ItemToXml<T>(T typedModel, List<Link> links)
        {
            IMediaConverter<T, XElement> converter;
            // todo: this container lookup might be inefficient for a List<T>???
            using (var container = SimpleWeb.Configuration.Container.BeginScope())
            {
                converter = container.Get<IMediaConverter<T, XElement>>();
            }

            var xml = converter.ToWireFormat(typedModel);
            if (links.Count > 0)
            {
                foreach (var link in links)
                {
                    xml.Add(link.ToXml());
                }
            }
            return xml;
        }
    }
}