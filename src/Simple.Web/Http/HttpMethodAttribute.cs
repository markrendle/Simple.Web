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
        /// <param name="httpMethod">The HTTP method to look for.</param>
        /// <param name="excludeInterfaces">If <c>true</c>, interfaces will not be included in the search.</param>
        /// <returns><c>null</c> if the attribute does not exist.</returns>
        public static HttpMethodAttribute Get(Type type, string httpMethod, bool excludeInterfaces = false)
        {
            var customAttribute = GetCustomAttributes(type, typeof (HttpMethodAttribute), true)
                .Cast<HttpMethodAttribute>()
                .FirstOrDefault(a => a.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase));
            return customAttribute ?? type.GetInterfaces().Select(i => Get(i, httpMethod)).FirstOrDefault(a => a != null);
        }

        public static Type GetAttributedType(Type type, string httpMethod)
        {
            if (TypeHasAttribute(type, httpMethod))
            {
                return type;
            }

            return type.GetInterfaces().FirstOrDefault(i => TypeHasAttribute(i, httpMethod));
        }

        private static bool TypeHasAttribute(Type type, string httpMethod)
        {
            return GetCustomAttributes(type, typeof (HttpMethodAttribute))
                .Cast<HttpMethodAttribute>()
                .Any(a => a.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the entry-point method name for a handler type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="httpMethod">The HTTP method to match against the type.</param>
        /// <returns>The value of the <see cref="Method"/> property, or <c>null</c> if the attribute is not applied to the type.</returns>
        public static MethodInfo GetMethod(Type type, string httpMethod)
        {
            var attr = Get(type, httpMethod);
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
            var isAppliedTo = Attribute.IsDefined(type, typeof (HttpMethodAttribute), true) ||
                              type.GetInterfaces().Any(i => Attribute.IsDefined(i, typeof(HttpMethodAttribute), false));
            return isAppliedTo;
        }

        public static bool Matches(Type type, string httpMethod)
        {
            var attribute = Get(type, httpMethod);
            if (attribute == null) return false;
            return attribute.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase);
        }
    }
}