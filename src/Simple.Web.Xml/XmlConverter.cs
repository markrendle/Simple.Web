namespace Simple.Web.Xml
{
    using System.Xml.Linq;

    public interface IXmlConverter
    {
        XElement ToXml(object value);
    }

    public abstract class XmlConverter<TModel> : IXmlConverter
    {
        public abstract TModel FromXml(XElement xml);

        public XElement ToXml(object value)
        {
            return ToXml((TModel)value);
        }

        public abstract XElement ToXml(TModel value);
    }
}