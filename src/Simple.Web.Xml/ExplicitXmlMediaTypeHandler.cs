using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Inflector;
using Simple.Web.DependencyInjection;
using Simple.Web.Helpers;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class ExplicitXmlMediaTypeHandler : MediaTypeHandlerBase<XElement>
    {
        private object _outputConverter;

        protected override void AddWireFormattedLinks(XElement wireFormattedItem, IEnumerable<Link> itemLinks)
        {
            if (itemLinks == null)
            {
                return;
            }
            foreach (Link link in itemLinks)
            {
                wireFormattedItem.Add(link.ToXml());
            }
        }

        protected override Task<T> FromWireFormat<T>(XElement wireFormat)
        {
            XmlConverter<T> converter;
            using (ISimpleContainerScope container = SimpleWeb.Configuration.Container.BeginScope())
            {
                converter = container.Get<XmlConverter<T>>();
            }
            return TaskHelper.Completed(converter.FromXml(wireFormat));
        }

        protected override Task<XElement> ReadInput(Stream inputStream)
        {
            return TaskHelper.Completed(XElement.Load(inputStream));
        }

        protected override XElement ToWireFormat(object item)
        {
            IXmlConverter converter;
            if (_outputConverter == null)
            {
                using (ISimpleContainerScope container = SimpleWeb.Configuration.Container.BeginScope())
                {
                    Type type = typeof (XmlConverter<>).MakeGenericType(item.GetType());
                    _outputConverter = converter = (IXmlConverter) container.Get(type);
                }
            }
            else
            {
                converter = (IXmlConverter) _outputConverter;
            }
            return converter.ToXml(item);
        }

        protected override XElement ToWireFormat<T>(T item)
        {
            XmlConverter<T> converter;
            if (_outputConverter == null)
            {
                using (ISimpleContainerScope container = SimpleWeb.Configuration.Container.BeginScope())
                {
                    _outputConverter = converter = container.Get<XmlConverter<T>>();
                }
            }
            else
            {
                converter = (XmlConverter<T>) _outputConverter;
            }
            return converter.ToXml(item);
        }

        protected override XElement WrapCollection(IList<XElement> collection, IEnumerable<Link> collectionLinks)
        {
            //todo should we wrap a single item?
            var xml = new XElement(collection[0].Name.LocalName.Pluralize());
            foreach (XElement element in collection)
            {
                xml.Add(element);
            }
            //todo add collection links?
            return xml;
        }

        protected override Task WriteOutput(XElement output, Stream outputStream)
        {
            output.Save(outputStream);
            return TaskHelper.Completed();
        }
    }
}