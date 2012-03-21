namespace Simple.Web.Razor
{
    using System;

    public class AmbiguousViewException : Exception
    {
        private readonly Type _modelType;

        public AmbiguousViewException(Type modelType) : base(string.Format("More than one View present for Model type {0}", modelType.Name))
        {
            _modelType = modelType;
        }

        public Type ModelType
        {
            get { return _modelType; }
        }
    }
}