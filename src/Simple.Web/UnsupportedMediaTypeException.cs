namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Thrown when a client offers or requests a Content-Type the system is unable to deal with.
    /// </summary>
    [Serializable]
    public sealed class UnsupportedMediaTypeException : HttpException
    {
        private readonly ReadOnlyCollection<string> _contentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedMediaTypeException"/> class.
        /// </summary>
        /// <param name="contentType">The Content-Type.</param>
        public UnsupportedMediaTypeException(string contentType)
            : base(415, "Requested type(s) not available")
        {
            _contentTypes = new ReadOnlyCollection<string>(new[] { contentType });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedMediaTypeException"/> class.
        /// </summary>
        /// <param name="contentTypes">An entire list of possible Content-Type values with which the system is still not able to deal.</param>
        public UnsupportedMediaTypeException(IList<string> contentTypes)
            : base(415, "Requested type(s) not available")
        {
            _contentTypes = new ReadOnlyCollection<string>(contentTypes);
        }

        /// <summary>
        /// Gets the content types.
        /// </summary>
        public ReadOnlyCollection<string> ContentTypes
        {
            get { return _contentTypes; }
        }

        /// <summary>
        /// Gets information about the exception and adds it to the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that holds the contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ContentTypes", _contentTypes);
        }
    }
}