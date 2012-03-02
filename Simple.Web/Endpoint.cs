using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    public abstract class GetEndpoint<TResponse> : IEndpoint
    {
        public abstract string UriTemplate { get; }
        public object Run()
        {
            return Get();
        }

        protected abstract TResponse Get();
    }

    public abstract class PostEndpoint<TRequest,TResponse> : IEndpoint
    {
        public abstract string UriTemplate { get; }
        public object Run()
        {
            return Get();
        }

        public TRequest Model { get; set; }
        protected abstract TResponse Get();
    }

    public interface IEndpoint
    {
        string UriTemplate { get; }
        object Run();
    }
}
