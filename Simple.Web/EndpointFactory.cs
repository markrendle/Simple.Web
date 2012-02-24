using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    class EndpointFactory
    {
        private EndpointFactory()
        {
            
        }

        private static EndpointFactory _instance;
        public static EndpointFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EndpointFactory();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        private readonly Dictionary<string, Type> _getEndpointTypes = new Dictionary<string, Type>();

        private void Initialize()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var exportedType in assembly.GetExportedTypes().Where(type => typeof(GetEndpoint).IsAssignableFrom(type) && !type.IsAbstract))
                {
                    var instance = Activator.CreateInstance(exportedType) as GetEndpoint;
                    if (instance != null)
                    {
                        _getEndpointTypes.Add(instance.UriTemplate, exportedType);
                    }
                }
            }
        }

        public GetEndpoint GetEndpoint(string absolutePath)
        {
            if (_getEndpointTypes.ContainsKey(absolutePath))
            {
                return (GetEndpoint)Activator.CreateInstance(_getEndpointTypes[absolutePath]);
            }
            return null;
        }
    }
}
