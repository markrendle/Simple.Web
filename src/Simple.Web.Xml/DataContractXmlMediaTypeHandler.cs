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
    public class DataContractXmlMediaTypeHandler : MediaTypeHandlerBase<XElement>
    {
        protected override Task<XElement> ReadInput(Stream inputStream)
        {
            return TaskHelper.Completed(XElement.Load(inputStream));
        }

        protected override Task<T> FromWireFormat<T>(XElement wireFormat)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(T));
            var xmlReader = wireFormat.CreateReader();
            var obj = dataContractSerializer.ReadObject(xmlReader);
            return TaskHelper.Completed((T)obj);
        }

        private DataContractSerializer _outputSerializer;

        protected override XElement ToWireFormat<T>(T item, IEnumerable<Link> itemLinks)
        {
            if (_outputSerializer == null)
            {
                _outputSerializer = new DataContractSerializer(typeof (T));
            }
            var stringBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(stringBuilder);
            _outputSerializer.WriteObject(xmlWriter, item);
            xmlWriter.Flush();
            var xml = XElement.Parse(stringBuilder.ToString());
            foreach (var link in itemLinks)
            {
                xml.Add(link.ToXml());
            }
            return xml;
        }

        protected override XElement WrapCollection(IList<XElement> collection, IEnumerable<Link> collectionLinks)
        {
            var xml = new XElement(collection[0].Name.LocalName.Pluralize());
            foreach (var element in collection)
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