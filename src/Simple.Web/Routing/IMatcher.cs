namespace Simple.Web.Routing
{
    using System.Collections.Generic;

    using Simple.Web.Hosting;

    internal interface IMatcher
    {
        IList<HandlerTypeInfo> Items { get; }

        MatcherCollection Matchers { get; }

        string Pattern { get; }

        IMatcher Add(string[] parts, int index, int priority);

        void AddTypeInfo(HandlerTypeInfo info);

        bool Match(string part, string template, int index, MatchData matchData);
    }
}