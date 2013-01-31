using System.Collections.Generic;
using Simple.Web.Hosting;

namespace Simple.Web.Routing
{
    internal interface IMatcher
    {
        void AddTypeInfo(HandlerTypeInfo info);
        IList<HandlerTypeInfo> Items { get; }
        string Pattern { get; }
        MatcherCollection Matchers { get; }
        bool Match(string part, string template, int index, MatchData matchData);
        IMatcher Add(string[] parts, int index, int priority);
    }
}