namespace Simple.Web.ContentTypeHandling
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public static class ObjectToDictionary
    {
        private static readonly ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>> Converters =
            new ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>>();

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var converter = Converters.GetOrAdd(obj.GetType(), CreateConverter);

            return converter(obj);
        }

        private static Func<object, IDictionary<string, object>> CreateConverter(Type type)
        {
            var toDictionaryMethod = type.GetMethod("ToDictionary", new Type[0]);
            if (toDictionaryMethod != null)
            {
                return obj => toDictionaryMethod.Invoke(obj, null) as IDictionary<string, object>;
            }

            return obj =>
                       {
                           var properties =
                               obj.GetType().GetProperties().Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                                   .ToList();
                           var dictionary = new Dictionary<string, object>(properties.Count);
                           foreach (var property in properties)
                           {
                               dictionary.Add(property.Name, property.GetValue(obj, null));
                           }
                           return dictionary;
                       };
        }
    }
}