namespace Simple.Web.Behaviors
{
    using System;

    public sealed class RequestBehaviorAttribute : BehaviorAttribute
    {
        public RequestBehaviorAttribute(Type implementingType) : base(implementingType)
        {
        }

        public static RequestBehaviorAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof (RequestBehaviorAttribute)) as RequestBehaviorAttribute;
        }
    }
}