using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Inflector;
using Simple.Web.Helpers;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class DataContractXmlMediaTypeHandler : MediaTypeHandlerBase<XElement>
    {
        private DataContractSerializer _outputSerializer;

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
            var dataContractSerializer = new DataContractSerializer(typeof (T));
            XmlReader xmlReader = wireFormat.CreateReader();
            object obj = dataContractSerializer.ReadObject(xmlReader);
            return TaskHelper.Completed((T) obj);
        }

        protected override Task<XElement> ReadInput(Stream inputStream)
        {
            return TaskHelper.Completed(XElement.Load(inputStream));
        }

        protected override XElement ToWireFormat(object item)
        {
            return ToWireFormat(item.GetType(), item);
        }

        protected override XElement ToWireFormat<T>(T item)
        {
            return ToWireFormat(typeof (T), item);
        }

        private XElement ToWireFormat(Type itemType, object item)
        {
            if (_outputSerializer == null)
            {
                _outputSerializer = new DataContractSerializer(itemType);
            }
            var stringBuilder = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);
            _outputSerializer.WriteObject(xmlWriter, item);
            xmlWriter.Flush();
            return XElement.Parse(stringBuilder.ToString());
        }

        protected override XElement WrapCollection(IList<XElement> collection, IEnumerable<Link> collectionLinks)
        {
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