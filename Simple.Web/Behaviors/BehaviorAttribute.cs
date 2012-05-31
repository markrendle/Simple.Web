namespace Simple.Web.Behaviors
{
    using System;

    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public abstract class BehaviorAttribute : Attribute
    {
        private readonly Type _implementingType;

        protected BehaviorAttribute(Type implementingType)
        {
            _implementingType = implementingType;
        }

        public Type ImplementingType
        {
            get { return _implementingType; }
        }

        public Priority Priority { get; set; }
    }
}