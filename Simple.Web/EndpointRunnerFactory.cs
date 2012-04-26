using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using System.Collections.Concurrent;
    using CodeGeneration;

    public class EndpointRunnerFactory
    {
        private readonly ConcurrentDictionary<Type, Action<object, IContext>> _cache =
            new ConcurrentDictionary<Type, Action<object, IContext>>();

        private readonly ConcurrentDictionary<Type, AsyncRunner> _asyncCache =
            new ConcurrentDictionary<Type, AsyncRunner>();

        public static readonly EndpointRunnerFactory Instance = new EndpointRunnerFactory();

        private EndpointRunnerFactory()
        {
        }

        public Action<object, IContext> Get(Type endpointType)
        {
            return _cache.GetOrAdd(endpointType, t => new EndpointRunnerBuilder(t).BuildRunner());
        }

        public AsyncRunner GetAsync(Type endpointType)
        {
            return _asyncCache.GetOrAdd(endpointType, t => new EndpointRunnerBuilder(t).BuildAsyncRunner());
        }
    }
}
