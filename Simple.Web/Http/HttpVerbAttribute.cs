namespace Simple.Web.Http
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Specifies which HTTP verb (e.g. GET, POST, HEAD) a Handler interface deals with.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class HttpVerbAttribute : Attribute
    {
        private readonly string _verb;
        private readonly string _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpVerbAttribute"/> class.
        /// </summary>
        /// <param name="verb">The HTTP verb.</param>
        public HttpVerbAttribute(string verb) : this(verb, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpVerbAttribute"/> class.
        /// </summary>
        /// <param name="verb">The HTTP verb.</param>
        /// <param name="method">The name of the entry-point method in the type.</param>
        public HttpVerbAttribute(string verb, string method)
        {
            _verb = verb;
            _method = method ?? char.ToUpperInvariant(verb[0]) + verb.Substring(1).ToLowerInvariant();
        }

        /// <summary>
        /// Gets the entry-point method name.
        /// </summary>
        public string Method
        {
            get { return _method; }
        }

        /// <summary>
        /// Gets the HTTP verb.
        /// </summary>
        public string Verb
        {
            get { return _verb; }
        }

        /// <summary>
        /// Gets the <see cref="HttpVerbAttribute"/> specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>null</c> if the attribute does not exist.</returns>
        public static HttpVerbAttribute Get(Type type)
        {
            return GetCustomAttribute(type, typeof (HttpVerbAttribute)) as HttpVerbAttribute;
        }

        /// <summary>
        /// Gets the entry-point method name for a handler type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type)
        {
            var attr = type.GetInterfaces().Select(Get).FirstOrDefault(a => a != null);
            return attr == null ? null : type.GetMethod(attr.Method);
        }

        /// <summary>
        /// Determines whether the <see cref="HttpVerbAttribute"/> is applied to the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if <see cref="HttpVerbAttribute"/> is applied to the specified type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAppliedTo(Type type)
        {
            return GetCustomAttribute(type, typeof (HttpVerbAttribute)) != null;
        }
    }
}