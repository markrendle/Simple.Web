namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Behaviors;
    using Helpers;
    using Http;

    class RequestBehaviorInfo : BehaviorInfo
    {
        private static List<RequestBehaviorInfo> _cache;

        public RequestBehaviorInfo(Type behaviorType, Type implementingType, Priority priority) : base(behaviorType, implementingType, priority)
        {
        }

        public static IEnumerable<RequestBehaviorInfo> GetInPriorityOrder()
        {
            if (_cache == null)
            {
                var list = FindRequestBehaviorTypes().OrderBy(t => t.Priority).ToList();
                Interlocked.CompareExchange(ref _cache, list, null);
            }

            return _cache;
        }

        private static IEnumerable<RequestBehaviorInfo> FindRequestBehaviorTypes()
        {
            return
                FindBehaviorTypes<RequestBehaviorAttribute, RequestBehaviorInfo>(
                    (behaviorType, implementingType, priority) => new RequestBehaviorInfo(behaviorType, implementingType, priority));
        }
    }
}