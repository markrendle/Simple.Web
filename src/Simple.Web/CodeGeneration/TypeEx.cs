using System;

namespace Simple.Web.CodeGeneration
{
    internal static class TypeEx
    {
        internal static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }
    }
}