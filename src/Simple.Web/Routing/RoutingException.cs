namespace Simple.Web.Routing
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when an error occurs in the URI routing system.
    /// </summary>
    [Serializable]
    public class RoutingException : Exception
    {
        private readonly string _url;
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="url">The URL that triggered the exception.</param>
        public RoutingException(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="url">The URL that triggered the exception.</param>
        /// <param name="message">The message.</param>
        public RoutingException(string url, string message) : base(message)
        {
            _url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="url">The URL that triggered the exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RoutingException(string url, string message, Exception inner) : base(message, inner)
        {
            _url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected RoutingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            _url = info.GetString("Url");
        }

        /// <summary>
        /// Gets the URL that triggered the exception.
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        ///   
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        ///   </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Url", Url);
        }
    }
}