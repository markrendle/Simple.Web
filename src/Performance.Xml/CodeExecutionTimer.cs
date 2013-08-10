using System;
using System.Diagnostics;

namespace Performance.Xml
{
    internal class CodeExecutionTimer
    {
        public static double Average(int iterations, Action timedAction)
        {
            return Average(iterations, timedAction, () => { });
        }

        public static double Average(int iterations, Action timedAction, Action nonTimedSetup)
        {
            var timer = new Stopwatch();

            //force jit
            nonTimedSetup();
            timedAction();

            var totalTime = new TimeSpan(0);
            for (int i = 0; i < iterations; i++)
            {
                // Give the code as good a chance as possible of avoiding garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                nonTimedSetup();
                timer.Start();
                timedAction();
                timer.Stop();
                totalTime += timer.Elapsed;
                timer.Reset();
            }
            return totalTime.TotalSeconds/iterations;
        }
    }
}