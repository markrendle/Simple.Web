namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class HandlerInfo
    {
        private readonly Type _handlerType;
        private readonly string _httpMethod;
        private readonly IDictionary<string, string> _variables;

        public HandlerInfo(Type handlerType, string httpMethod)
            : this(handlerType, new Dictionary<string, string>(), httpMethod)
        {
        }

        public HandlerInfo(Type handlerType, IDictionary<string, string> variables, string httpMethod)
        {
            if (handlerType == null) throw new ArgumentNullException("handlerType");
            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            _handlerType = handlerType;
            _variables = variables ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _httpMethod = httpMethod;
        }

        public string HttpMethod
        {
            get { return _httpMethod; }
        }

        public IDictionary<string, string> Variables
        {
            get { return _variables; }
        }

        public Type HandlerType
        {
            get { return _handlerType; }
        }

        public bool RequiresAuthentication
        {
            get { return typeof (IRequireAuthentication).IsAssignableFrom(_handlerType); }
        }

        public Type InputType
        {
            get { return GetInterfaceGenericType(typeof (IInput<>)); }
        }

        public Type OutputType
        {
            get { return GetInterfaceGenericType(typeof (IOutput<>)); }
        }

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