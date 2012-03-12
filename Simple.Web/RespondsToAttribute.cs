namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RespondsToAttribute : Attribute
    {
        private readonly string[] _acceptTypes;

        public RespondsToAttribute(params string[] acceptTypes)
        {
            _acceptTypes = acceptTypes;
        }

        public ReadOnlyCollection<string> AcceptTypes
        {
            get { return Array.AsReadOnly(_acceptTypes); }
        }

        internal static IEnumerable<RespondsToAttribute> Get(Type type)
        {
            return GetCustomAttributes(type, typeof (RespondsToAttribute))
                .Cast<RespondsToAttribute>();
        }
    }
}