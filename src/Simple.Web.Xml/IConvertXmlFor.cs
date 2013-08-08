namespace Simple.Web.Xml
{
    using System.Xml.Linq;

    /// <summary>
    ///     An interface for converting a model to and from a wire format.
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <remarks>
    /// </remarks>
    public interface IConvertXmlFor<TModel>
    {
        /// <summary>
        ///     Hydrates a new instance of {TModel} from the state present in the xml representation.
        /// </summary>
        /// <param name="xml"> The format from the wire. </param>
        /// <returns></returns>
        /// <remarks></remarks>
        TModel FromXml(XElement xml);

        /// <summary>
        ///     Saves the state of {TModel} in xml} form.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns></returns>
        /// <remarks></remarks>
        XElement ToXml(TModel value);
    }
}