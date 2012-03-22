namespace Simple.Web.Razor
{
    using System;

    public class ViewNotFoundException : Exception
    {
        private readonly Type _modelType;

        public ViewNotFoundException() : base("No view found.")
        {
        }

        public ViewNotFoundException(Type modelType) : base(string.Format("No View present for Model type {0}", modelType.Name))
        {
            _modelType = modelType;
        }

        public Type ModelType
        {
            get { return _modelType; }
        }
    }
}