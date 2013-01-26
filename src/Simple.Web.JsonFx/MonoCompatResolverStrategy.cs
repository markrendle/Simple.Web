using System.Reflection;
using JsonFx.Serialization.Resolvers;

namespace Simple.Web.JsonFx
{
    public class MonoCompatResolverStrategy : PocoResolverStrategy
    {
        public override bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
        {
            return base.IsPropertyIgnored(member, isImmutableType) || member.GetIndexParameters().Length > 0;
        }
    }
}