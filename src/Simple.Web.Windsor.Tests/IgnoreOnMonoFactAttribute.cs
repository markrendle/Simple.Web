namespace Simple.Web.Windsor.Tests
{
    using System;

    using Xunit;

    public class IgnoreOnMonoFactAttribute : FactAttribute
    {
        public IgnoreOnMonoFactAttribute()
        {
            if (IsRunningOnMono())
            {
                Skip = "Ignored on Mono";
            }
        }

        /// <summary>
        /// Determine if runtime is Mono.
        /// Taken from http://stackoverflow.com/questions/721161
        /// </summary>
        /// <returns>True if being executed in Mono, false otherwise.</returns>
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}