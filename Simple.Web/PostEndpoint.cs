namespace Simple.Web
{
    using System;

    public abstract class PostEndpoint<TRequest,TResponse> : IInputEndpoint<TRequest>, IOutputEndpoint<TResponse>
    {
        public Status Run()
        {
            return Get();
        }

        public TRequest Input { protected get; set; }

        Type IInputEndpoint.InputType
        {
            get { return typeof (TRequest); }
        }

        public TResponse Output { get; protected set; }

        public Type OutputType
        {
            get { return typeof (TResponse); }
        }

        object IOutputEndpoint.Output { get { return Output; } }
        protected abstract Status Get();

        object IInputEndpoint.Input
        {
            set { Input = (TRequest) value; }
        }
    }
}