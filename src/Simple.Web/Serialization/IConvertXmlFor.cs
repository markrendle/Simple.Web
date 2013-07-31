using System.Xml.Linq;

namespace Simple.Web.Serialization
{
	/// <summary>
	///     An interface for converting to and from Linq2Xml objects.
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	/// <remarks>
	/// </remarks>
	public interface IConvertXmlFor<T>
	{
		/// <summary>
		///     Hydrates a new instance of {T} from the state present in the xml representation.
		/// </summary>
		/// <param name="element"> The element. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		T FromXml(XElement element);

		/// <summary>
		///     Hydrates loadThis from the state present in the xml representation.
		/// </summary>
		/// <param name="element"> The element. </param>
		/// <param name="loadThis"> The load this. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		T FromXml(XElement element, T loadThis);

		/// <summary>
		///     Saves the state of {T} in xml form using SaveState.Complete.
		/// </summary>
		/// <param name="value"> The value. </param>
		/// <param name="name"> The name. </param>
		/// <returns> XElement. </returns>
		XElement ToXml(T value, XName name = null);
	}
}