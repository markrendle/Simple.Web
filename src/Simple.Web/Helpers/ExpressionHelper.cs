namespace Simple.Web.Helpers
{
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ExpressionHelper
    {
        internal static bool TryGetValue(Expression expression, out object value)
        {
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                value = constantExpression.Value;
                return true;
            }

            var memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                if (memberExpression.Expression == null) // Static
                {
                    return TryGetValue(memberExpression.Member, out value);
                }
                object memberValue;
                constantExpression = memberExpression.Expression as ConstantExpression;
                if (constantExpression != null)
                {
                    memberValue = constantExpression.Value;
                }
                else
                {
                    if (!TryGetValue(memberExpression.Expression, out memberValue))
                    {
                        value = null;
                        return false;
                    }
                }
                if (memberValue == null)
                {
                    value = null;
                    return false;
                }
                var members = memberValue.GetType()
                                         .GetMember(memberExpression.Member.Name,
                                                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (members.Length != 1)
                {
                    value = null;
                    return false;
                }
                return TryGetValue(members[0], out value, memberValue);
            }

            value = null;
            return false;
        }

        private static bool TryGetValue(MemberInfo memberInfo, out object value, object memberValue = null)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(memberValue, null);
                return true;
            }
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                value = fieldInfo.GetValue(memberValue);
                return true;
            }
            value = null;
            return false;
        }
    }
}