namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Simple.Web.Behaviors;

    internal class ResponseBehaviorInfo : BehaviorInfo
    {
        private static List<ResponseBehaviorInfo> _cache;

        public ResponseBehaviorInfo(Type behaviorType, Type implementingType, Priority priority)
            : base(behaviorType, implementingType, priority)
        {
        }

        public static IEnumerable<ResponseBehaviorInfo> GetInPriorityOrder(params ResponseBehaviorInfo[] defaults)
        {
            if (_cache == null)
            {
                var list = FindResponseBehaviorTypes().Concat(defaults).OrderBy(r => r.Priority).ToList();
                Interlocked.CompareExchange(ref _cache, list, null);
            }

            return _cache;
        }

        private static IEnumerable<ResponseBehaviorInfo> FindResponseBehaviorTypes()
        {
            return FindBehaviorTypes<ResponseBehaviorAttribute, ResponseBehaviorInfo>((t, i, p) => new ResponseBehaviorInfo(t, i, p));
        }
    }
}