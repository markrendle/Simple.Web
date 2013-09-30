namespace Simple.Web.Routing
{
    using System;

    internal class MatcherFactory
    {
        public static IMatcher Create(string pattern)
        {
            if (pattern.Contains("{"))
            {
                if (pattern.StartsWith("{") && pattern.EndsWith("}") && pattern.IndexOf('{', 1) == -1)
                {
                    return new SingleValueMatcher(pattern);
                }
                throw new NotSupportedException("Complex patterns not supported yet.");
            }
            return new StaticMatcher(pattern);
        }
    }
}