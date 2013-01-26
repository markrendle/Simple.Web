namespace Simple.Web.Http
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Specifies which HTTP method (e.g. GET, POST, HEAD) a Handler interface deals with.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class HttpMethodAttribute : Attribute
    {
        private readonly string _httpMethod;
        private readonly string _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMethodAttribute"/> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP Method.</param>
        public HttpMethodAttribute(string httpMethod) : this(httpMethod, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMethodAttribute"/> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP Method.</param>
        /// <param name="method">The name of the entry-point method in the type.</param>
        public HttpMethodAttribute(string httpMethod, string method)
        {
            _httpMethod = httpMethod;
            _method = method ?? char.ToUpperInvariant(httpMethod[0]) + httpMethod.Substring(1).ToLowerInvariant();
        }

        /// <summary>
        /// Gets the entry-point method name.
        /// </summary>
        public string Method
        {
            get { return _method; }
        }

        /// <summary>
        /// Gets the HTTP Method.
        /// </summary>
        public string HttpMethod
        {
            get { return _httpMethod; }
        }

        /// <summary>
        /// Gets the <see cref="HttpMethodAttribute"/> specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>null</c> if the attribute does not exist.</returns>
        public static HttpMethodAttribute Get(Type type)
        {
            return (GetCustomAttribute(type, typeof(HttpMethodAttribute)) ?? type.GetInterfaces().Select(Get).FirstOrDefault()) as HttpMethodAttribute;
        }

        /// <summary>
        /// Gets the entry-point method name for a handler type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The value of the <see cref="Method"/> property, or <c>null</c> if the attribute is not applied to the type.</returns>
        public static MethodInfo GetMethod(Type type)
        {
            var attr = type.GetInterfaces().Select(Get).FirstOrDefault(a => a != null);
            return attr == null ? null : type.GetMethod(attr.Method);
        }

        /// <summary>
        /// Determines whether the <see cref="HttpMethodAttribute"/> is applied to the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if <see cref="HttpMethodAttribute"/> is applied to the specified type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAppliedTo(Type type)
        {
            var isAppliedTo = GetCustomAttribute(type, typeof (HttpMethodAttribute), true) != null || type.GetInterfaces().Select(Get).FirstOrDefault() != null;
            return isAppliedTo;
        }
    }
}