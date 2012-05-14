namespace Simple.Web.Razor
{
    using System;

    public class ViewNotFoundException : Exception
    {
        private readonly Type _handlerType;
        private readonly Type _modelType;

        public ViewNotFoundException() : base("No view found.")
        {
        }

        public ViewNotFoundException(Type handlerType, Type modelType) : base(MakeMessage(handlerType, modelType))
        {
            _handlerType = handlerType;
            _modelType = modelType;
        }

        public Type HandlerType
        {
            get { return _handlerType; }
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        private static string MakeMessage(Type handlerType, Type modelType)
        {
            if (handlerType == null)
                return string.Format("No View present for Model type {0}", modelType.Name);
            if (modelType == null)
                return string.Format("No View present for Handler type {0}", handlerType.Name);
            return string.Format("No View present for Handler type {0}, Model type {1}", handlerType.Name, modelType.Name);
        }
    }
}