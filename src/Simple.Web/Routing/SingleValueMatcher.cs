﻿namespace Simple.Web.Routing
{
    internal class SingleValueMatcher : MatcherBase
    {
        private readonly string _trimmed;

        public SingleValueMatcher(string pattern)
            : base(pattern, 1)
        {
            _trimmed = pattern.Trim('{', '}');
        }

        protected override bool OnMatch(string part, MatchData matchData)
        {
            matchData.SetVariable(_trimmed, part);
            return true;
        }
    }
}