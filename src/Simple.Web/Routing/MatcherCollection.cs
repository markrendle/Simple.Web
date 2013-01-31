using System;
using System.Collections.ObjectModel;

namespace Simple.Web.Routing
{
    class MatcherCollection : KeyedCollection<string, IMatcher>
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