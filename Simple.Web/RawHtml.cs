namespace Simple.Web
{
    /// <summary>
    /// Wraps a string and tells the framework that it should be treated as raw HTML.
    /// </summary>
    /// <remarks>This class is instantiated by implicit casting from <see cref="string"/> instances.</remarks>
    public class RawHtml
    {
        private readonly string _html;

        internal RawHtml(string html)
        {
            _html = html;
        }

        /// <summary>
        /// Gets the HTML.
        /// </summary>
        public string Html
        {
            get { return _html; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _html;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Simple.Web.RawHtml"/>.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator RawHtml(string html)
        {
            return new RawHtml(html);
        }
    }
}