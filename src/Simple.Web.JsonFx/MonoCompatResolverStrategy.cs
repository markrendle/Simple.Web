namespace Simple.Web.JsonFx
{
    using System.Reflection;

    using global::JsonFx.Serialization.Resolvers;

    public class MonoCompatResolverStrategy : PocoResolverStrategy
    {
        public override bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
        {
            return base.IsPropertyIgnored(member, isImmutableType) || member.GetIndexParameters().Length > 0;
        }
    }
}