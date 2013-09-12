using System.Xml.Linq;

namespace Simple.Web.Xml
{
    public interface IXmlConverter
    {
        XElement ToXml(object value);
    }

    public abstract class XmlConverter<TModel> : IXmlConverter
    {
        public XElement ToXml(object value)
        {
            return ToXml((TModel) value);
        }

        public abstract TModel FromXml(XElement xml);

        public abstract XElement ToXml(TModel value);
    }
}