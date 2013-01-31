namespace Simple.Web.JsonNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class LinkConverter
    {
        internal static readonly MethodInfo WritePropertyName = typeof(JsonWriter).GetMethod("WritePropertyName", new[] { typeof(string) });
        internal static readonly MethodInfo Serialize = typeof(JsonSerializer).GetMethod("Serialize", new[] { typeof(JsonWriter), typeof(object) });
        private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = new ConcurrentDictionary<Type, JsonConverter>();
        private static readonly ConcurrentDictionary<Type, JsonConverter[]> ConverterSets = new ConcurrentDictionary<Type, JsonConverter[]>();

        public static JsonConverter Create(Type type, Func<object, IEnumerable<object>> linkEnumerator, IContractResolver contractResolver)
        {
            return Converters.GetOrAdd(type, t => Build(t, linkEnumerator, contractResolver));
        }

        public static JsonConverter[] CreateForGraph(Type type, HashSet<Type> knownTypes, Func<object, IEnumerable<object>> linkEnumerator, IContractResolver contractResolver)
        {
            return ConverterSets.GetOrAdd(type, t => CreateForGraphImpl(t, knownTypes, linkEnumerator, contractResolver));
        }

        private static JsonConverter[] CreateForGraphImpl(Type type, HashSet<Type> knownTypes, Func<object, IEnumerable<object>> linkEnumerator, IContractResolver contractResolver)
        {
            var list = new List<JsonConverter> { Create(type, linkEnumerator, contractResolver) };
            Add(type, list, knownTypes, linkEnumerator, new HashSet<Type>(), contractResolver);
            return list.ToArray();
        }

        private static void Add(Type type, IList<JsonConverter> converters, HashSet<Type> knownTypes, Func<object, IEnumerable<object>> linkEnumerator, HashSet<Type> done, IContractResolver contractResolver)
        {
            if (knownTypes.Contains(type))
            {
                converters.Add(Create(type, linkEnumerator, contractResolver));
            }
            done.Add(type);
            foreach (var property in type.GetProperties().Where(p => (!Ignore(p.PropertyType)) && (!done.Contains(p.PropertyType))))
            {
                Add(property.PropertyType, converters, knownTypes, linkEnumerator, done, contractResolver);
            }
        }

        internal static bool Ignore(Type type)
        {
            return type.IsPrimitive || type.IsArray || type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) || type.Name == "Nullable`1";
        }

        private static JsonConverter Build(Type type, Func<object, IEnumerable<object>> linkEnumerator, IContractResolver contractResolver)
        {
            var enumerable = type.GetInterface(typeof (IEnumerable<>).FullName);
            if (enumerable != null)
            {
                return Build(enumerable.GetGenericArguments().Single(), linkEnumerator, contractResolver);
            }
            var concreteType = typeof(LinkConverter<>).MakeGenericType(type);
            var newMethod = concreteType.GetMethod("New", BindingFlags.Static | BindingFlags.Public);
            return (JsonConverter)newMethod.Invoke(null, new object[] {linkEnumerator, contractResolver});
        }
    }

    
}