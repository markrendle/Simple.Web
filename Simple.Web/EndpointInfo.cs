namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    class EndpointInfo
    {
        private readonly Type _endpointType;
        private readonly string _httpMethod;
        private readonly IDictionary<string, string> _variables;

        public EndpointInfo(Type endpointType, string httpMethod) : this(endpointType, new Dictionary<string, string>(), httpMethod)
        {
        }

        public EndpointInfo(Type endpointType, IDictionary<string, string> variables, string httpMethod)
        {
            _endpointType = endpointType;
            _variables = variables;
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
    }
}