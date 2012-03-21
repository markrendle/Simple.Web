namespace Simple.Web
{
    using System;

    public abstract class GetEndpoint<TResponse> : IOutputEndpoint<TResponse>
    {
        public Status Run()
        {
            return Get();
        }

        public string UriTemplate
        {
            get { throw new System.NotImplementedException(); }
        }

        public TResponse Output { get; protected set; }

        public Type OutputType
        {
            get { return typeof (TResponse); }
        }

        object IOutputEndpoint.Output
        {
            get { return Output; }
        }

        protected abstract Status Get();
    }
}