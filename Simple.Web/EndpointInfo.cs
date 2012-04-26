namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class EndpointInfo
    {
        private readonly Type _endpointType;
        private readonly string _httpMethod;
        private readonly IDictionary<string, string> _variables;

        public EndpointInfo(Type endpointType, string httpMethod)
            : this(endpointType, new Dictionary<string, string>(), httpMethod)
        {
        }

        public EndpointInfo(Type endpointType, IDictionary<string, string> variables, string httpMethod)
        {
            if (endpointType == null) throw new ArgumentNullException("endpointType");
            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            _endpointType = endpointType;
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

        public Type EndpointType
        {
            get { return _endpointType; }
        }

        public bool RequiresAuthentication
        {
            get { return typeof (IRequireAuthentication).IsAssignableFrom(_endpointType); }
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
            get { return HttpVerbAttribute.GetMethod(_endpointType).ReturnType == typeof (Task<Status>); }
        }

        private Type GetInterfaceGenericType(Type genericType)
        {
            var genericInterface = _endpointType.GetInterface(genericType.Name);
            if (genericInterface == null)
            {
                return null;
            }
            return genericInterface.GetGenericArguments().FirstOrDefault();
        }
    }
}