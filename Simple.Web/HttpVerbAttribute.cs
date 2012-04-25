namespace Simple.Web
{
    using System;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class HttpVerbAttribute : Attribute
    {
        private readonly string _verb;
        private readonly string _method;

        public HttpVerbAttribute(string verb) : this(verb, null)
        {
        }

        public HttpVerbAttribute(string verb, string method)
        {
            _verb = verb;
            _method = method ?? char.ToUpperInvariant(verb[0]) + verb.Substring(1).ToLowerInvariant();
        }

        public string Method
        {
            get { return _method; }
        }

        public string Verb
        {
            get { return _verb; }
        }

        public static HttpVerbAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof (HttpVerbAttribute)) as HttpVerbAttribute;
        }

        public static MethodInfo GetMethod(Type type)
        {
            var attr = type.GetInterfaces().Select(Get).FirstOrDefault(a => a != null);
            return attr == null ? null : type.GetMethod(attr.Method);
        }

        public static bool IsAppliedTo(Type type)
        {
            return GetCustomAttribute(type, typeof (HttpVerbAttribute)) != null;
        }
    }
}