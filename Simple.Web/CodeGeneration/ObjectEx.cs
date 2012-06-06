using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.Web.CodeGeneration
{
    internal static class ObjectEx
    {
        private static readonly ConcurrentDictionary<Type, Func<object, IDictionary<string, string>>> Converters =
            new ConcurrentDictionary<Type, Func<object, IDictionary<string, string>>>();

        public static IDictionary<string, string> ObjectToDictionary(this object obj)
        {
            if (obj == null) return new Dictionary<string, string>();

            return Converters.GetOrAdd(obj.GetType(), MakeToDictionaryFunc)(obj);
        }

        private static Func<object, IDictionary<string, string>> MakeToDictionaryFunc(Type type)
        {
            var param = Expression.Parameter(typeof(object));
            var typed = Expression.Variable(type);
            var newDict = Expression.New(typeof(Dictionary<string, string>));
            var listInit = Expression.ListInit(newDict, GetElementInitsForType(type, typed));

            var block = Expression.Block(new[] { typed },
                                         Expression.Assign(typed, Expression.Convert(param, type)),
                                         listInit);

            return Expression.Lambda<Func<object, IDictionary<String, string>>>(block, param).Compile();
        }

        static readonly MethodInfo DictionaryAddMethod = typeof(Dictionary<string, string>).GetMethod("Add");
        static readonly MethodInfo ToStringOrNullMethod = typeof(ObjectEx).GetMethod("ToStringOrNull", BindingFlags.Static | BindingFlags.NonPublic);

        static IEnumerable<ElementInit> GetElementInitsForType(Type type, Expression param)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .Select(p => PropertyToElementInit(p, param));
        }

        static ElementInit PropertyToElementInit(PropertyInfo propertyInfo, Expression instance)
        {
            return Expression.ElementInit(DictionaryAddMethod,
                                          Expression.Constant(propertyInfo.Name),
                                          Expression.Call(ToStringOrNullMethod,
                                                          Expression.Convert(Expression.Property(instance, propertyInfo), typeof(object))));
        }

        static string ToStringOrNull(object obj)
        {
            if (obj == null) return null;
            return obj.ToString();
        }

        internal static bool IsAnonymous(this object obj)
        {
            return obj.GetType().Namespace == null;
        }
    }
}