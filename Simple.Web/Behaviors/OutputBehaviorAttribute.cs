namespace Simple.Web.Behaviors
{
    using System;

    public sealed class OutputBehaviorAttribute : BehaviorAttribute
    {
        public OutputBehaviorAttribute(Type implementingType)
            : base(implementingType)
        {
        }

        public static OutputBehaviorAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof(OutputBehaviorAttribute)) as OutputBehaviorAttribute;
        }
    }
}