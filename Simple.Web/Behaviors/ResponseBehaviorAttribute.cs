namespace Simple.Web.Behaviors
{
    using System;

    public sealed class ResponseBehaviorAttribute : BehaviorAttribute
    {
        public ResponseBehaviorAttribute(Type implementingType)
            : base(implementingType)
        {
        }

        public static ResponseBehaviorAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof(ResponseBehaviorAttribute)) as ResponseBehaviorAttribute;
        }
    }
}