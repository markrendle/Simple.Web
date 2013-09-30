namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Simple.Web.Behaviors;

    internal class OutputBehaviorInfo : BehaviorInfo
    {
        private static List<OutputBehaviorInfo> _cache;

        public OutputBehaviorInfo(Type behaviorType, Type implementingType, Priority priority)
            : base(behaviorType, implementingType, priority)
        {
        }

        public static IEnumerable<OutputBehaviorInfo> GetInPriorityOrder()
        {
            if (_cache == null)
            {
                var list = FindOutputBehaviorTypes().OrderBy(t => t.Priority).ToList();
                Interlocked.CompareExchange(ref _cache, list, null);
            }

            return _cache;
        }

        private static IEnumerable<OutputBehaviorInfo> FindOutputBehaviorTypes()
        {
            return FindBehaviorTypes<OutputBehaviorAttribute, OutputBehaviorInfo>((t, i, p) => new OutputBehaviorInfo(t, i, p));
        }
    }
}