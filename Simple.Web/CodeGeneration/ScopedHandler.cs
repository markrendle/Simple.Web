using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Web.DependencyInjection;

namespace Simple.Web.CodeGeneration
{
    public class ScopedHandler: IScopedHandler
    {
        private ISimpleContainerScope _scope;
        public object Handler { get; private set; }

        public ScopedHandler(object handler, ISimpleContainerScope scope)
        {
            Handler = handler;
            _scope = scope;
        }

        public static IScopedHandler Create(object handler, ISimpleContainerScope scope)
        {
            return new ScopedHandler(handler, scope);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
