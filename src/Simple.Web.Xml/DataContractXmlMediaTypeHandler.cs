using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Simple.Web.Helpers;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class DataContractXmlMediaTypeHandler : XElementMediaTypeHandlerBase
    {
        private DataContractSerializer _outputSerializer;

        protected override Task<T> FromWireFormat<T>(XElement wireFormat)
        {
            var dataContractSerializer = new DataContractSerializer(typeof (T));
            XmlReader xmlReader = wireFormat.CreateReader();
            object obj = dataContractSerializer.ReadObject(xmlReader);
            return TaskHelper.Completed((T) obj);
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
    }
}