using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Simple.Web.DependencyInjection;
using Simple.Web.Helpers;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
    [MediaTypes(MediaType.Xml, "application/*+xml")]
    public class ExplicitXmlMediaTypeHandler : XElementMediaTypeHandlerBase
    {
        private object _outputConverter;

        protected override Task<T> FromWireFormat<T>(XElement wireFormat)
        {
            XmlConverter<T> converter;
            using (ISimpleContainerScope container = SimpleWeb.Configuration.Container.BeginScope())
            {
                converter = container.Get<XmlConverter<T>>();
            }
            return TaskHelper.Completed(converter.FromXml(wireFormat));
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
    }
}