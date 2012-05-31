namespace Simple.Web.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Http;
    using Simple.Web.Behaviors;

    /// <summary>
    /// Provides useful information about handlers.
    /// </summary>
    public sealed class HandlerInfo
    {
        private readonly Type _handlerType;
        private readonly string _httpMethod;
        private readonly IDictionary<string, string> _variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerInfo"/> class.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        public HandlerInfo(Type handlerType, string httpMethod)
            : this(handlerType, new Dictionary<string, string>(), httpMethod)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerInfo"/> class.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        /// <param name="variables">The variables.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        public HandlerInfo(Type handlerType, IDictionary<string, string> variables, string httpMethod)
        {
            if (handlerType == null) throw new ArgumentNullException("handlerType");
            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            _handlerType = handlerType;
            _variables = variables ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _httpMethod = httpMethod;
        }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public string HttpMethod
        {
            get { return _httpMethod; }
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IDictionary<string, string> Variables
        {
            get { return _variables; }
        }

        /// <summary>
        /// Gets the type of the handler.
        /// </summary>
        /// <value>
        /// The type of the handler.
        /// </value>
        public Type HandlerType
        {
            get { return _handlerType; }
        }

        /// <summary>
        /// Gets a value indicating whether the Handler requires authentication.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the Handler requires authentication; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresAuthentication
        {
            get { return typeof (IRequireAuthentication).IsAssignableFrom(_handlerType); }
        }

        /// <summary>
        /// Gets the type of the input.
        /// </summary>
        /// <value>
        /// The type of the input.
        /// </value>
        public Type InputType
        {
            get { return GetInterfaceGenericType(typeof (IInput<>)); }
        }

        /// <summary>
        /// Gets the type of the output.
        /// </summary>
        /// <value>
        /// The type of the output.
        /// </value>
        public Type OutputType
        {
            get { return GetInterfaceGenericType(typeof (IOutput<>)); }
        }

        /// <summary>
        /// Gets a value indicating whether the handler is asynchronous.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the handler is asynchronous; otherwise, <c>false</c>.
        /// </value>
        public bool IsAsync
        {
            get { return HttpVerbAttribute.GetMethod(_handlerType).ReturnType == typeof (Task<Status>); }
        }

        private Type GetInterfaceGenericType(Type genericType)
        {
            var genericInterface = _handlerType.GetInterface(genericType.Name);
            if (genericInterface == null)
            {
                return null;
            }
            return genericInterface.GetGenericArguments().FirstOrDefault();
        }
    }
}