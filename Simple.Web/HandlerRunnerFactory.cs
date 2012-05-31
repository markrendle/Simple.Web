using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using System.Collections.Concurrent;
    using CodeGeneration;
    using Http;

    public class HandlerRunnerFactory
    {
        private readonly ConcurrentDictionary<Type, Action<object, IContext>> _cache =
            new ConcurrentDictionary<Type, Action<object, IContext>>();

        private readonly ConcurrentDictionary<Type, AsyncRunner> _asyncCache =
            new ConcurrentDictionary<Type, AsyncRunner>();

        public static readonly HandlerRunnerFactory Instance = new HandlerRunnerFactory();

        private HandlerRunnerFactory()
        {
        }

        public Action<object, IContext> Get(Type handlerType)
        {
            return _cache.GetOrAdd(handlerType, t => new HandlerRunnerBuilder(t).BuildRunner());
        }

        public AsyncRunner GetAsync(Type handlerType)
        {
            return _asyncCache.GetOrAdd(handlerType, t => new HandlerRunnerBuilder(t).BuildAsyncRunner());
        }
    }
}
