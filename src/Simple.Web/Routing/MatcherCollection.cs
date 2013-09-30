namespace Simple.Web.Routing
{
    using System;
    using System.Collections.ObjectModel;

    internal class MatcherCollection : KeyedCollection<string, IMatcher>
    {
        public MatcherCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override string GetKeyForItem(IMatcher item)
        {
            return item.Pattern;
        }
    }
}