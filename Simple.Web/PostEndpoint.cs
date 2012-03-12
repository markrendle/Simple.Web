namespace Simple.Web
{
    using System;

    public abstract class PostEndpoint<TRequest,TResponse> : IInputEndpoint
    {
        public Status Run()
        {
            return Get();
        }

        public TRequest Input { get; set; }

        Type IInputEndpoint.InputType
        {
            get { return typeof (TRequest); }
        }

        public TResponse Output { get; protected set; }

        object IEndpoint.Output { get { return Output; } }
        protected abstract Status Get();

        object IInputEndpoint.Input
        {
            get { return Input; }
            set { Input = (TRequest) value; }
        }
    }
}