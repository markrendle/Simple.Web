namespace Simple.Web.MediaTypeHandling
{
    /// <summary>
    ///     An interface for converting a model to and from a wire format.
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TWireFormat">The wire format type</typeparam>
    /// <remarks>
    /// </remarks>
    public interface IMediaConverter<TModel, TWireFormat>
    {
        /// <summary>
        ///     Hydrates a new instance of {TModel} from the state present in the {TWireFormat} representation.
        /// </summary>
        /// <param name="wireFormat"> The format from the wire. </param>
        /// <returns></returns>
        /// <remarks></remarks>
        TModel FromWireFormat(TWireFormat wireFormat);

        /// <summary>
        ///     Hydrates loadThis from the state present in the {TWireFormat} representation.
        /// </summary>
        /// <param name="wireFormat"> The format from the wire. </param>
        /// <param name="loadThis"> The load this. </param>
        /// <returns><param name="loadThis"> with appropriate values set.</param></returns>
        /// <remarks></remarks>
        TModel FromWireFormat(TWireFormat wireFormat, TModel loadThis);

        /// <summary>
        ///     Saves the state of {TModel} in {TWireFormat} form.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns></returns>
        /// <remarks></remarks>
        TWireFormat ToWireFormat(TModel value);
    }
}