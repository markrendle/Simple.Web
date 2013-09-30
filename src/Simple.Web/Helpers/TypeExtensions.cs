namespace Simple.Web.Helpers
{
    using System;

    public static class TypeExtensions
    {
        public static bool IsJsonPrimitive(this Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type.IsEnum || type.IsNullable();
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}