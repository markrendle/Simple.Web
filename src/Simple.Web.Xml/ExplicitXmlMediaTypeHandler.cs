namespace Simple.Web.Xml
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Inflector;
    using Helpers;
    using Links;
    using MediaTypeHandling;

    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class ExplicitXmlMediaTypeHandler : MediaTypeHandlerBase<XElement>
    {
        private object _outputConverter;

        protected override Task<XElement> ReadInput(Stream inputStream)
        {
            return TaskHelper.Completed(XElement.Load(inputStream));
        }

        protected override Task<T> FromWireFormat<T>(XElement wireFormat)
        {
            IConvertXmlFor<T> converter;
            using (var container = SimpleWeb.Configuration.Container.BeginScope())
            {
                converter = container.Get<IConvertXmlFor<T>>();
            }
            return TaskHelper.Completed(converter.FromXml(wireFormat));
        }

        protected override XElement ToWireFormat<T>(T item, IEnumerable<Link> itemLinks)
        {
            IConvertXmlFor<T> converter;
            if (_outputConverter == null)
            {
                using (var container = SimpleWeb.Configuration.Container.BeginScope())
                {
                    _outputConverter = converter = container.Get<IConvertXmlFor<T>>();
                }
            }
            else
            {
                converter = (IConvertXmlFor<T>) _outputConverter;
            }

            var xml = converter.ToXml(item);
            if (itemLinks != null)
            {
                foreach (var link in itemLinks)
                {
                    xml.Add(link.ToXml());
                }
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