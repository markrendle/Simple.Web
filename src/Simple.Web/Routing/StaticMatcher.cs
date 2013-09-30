namespace Simple.Web.Routing
{
    using System;

    internal class StaticMatcher : MatcherBase
    {
        public StaticMatcher(string pattern)
            : base(pattern, 0)
        {
        }

        protected override bool OnMatch(string value, MatchData matchData)
        {
            return value.Equals(Pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}