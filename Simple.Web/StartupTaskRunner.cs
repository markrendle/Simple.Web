namespace Simple.Web
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public sealed class StartupTaskRunner
    {
        private static readonly Type StartupTaskType = typeof (IStartupTask);

        public void RunStartupTasks()
        {
            foreach (var type in ExportedTypeHelper.FromCurrentAppDomain(StartupTaskType.IsAssignableFrom))
            {
                CreateAndRunTask(type);
            }
        }

        private static void CreateAndRunTask(Type type)
        {
            try
            {
                TryRun(type);
            }
            catch (TargetInvocationException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (MethodAccessException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (MemberAccessException ex)
            {
                Trace.TraceError(ex.Message);
            }
            catch (TypeLoadException ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private static void TryRun(Type type)
        {
            var task = Activator.CreateInstance(type) as IStartupTask;
            if (task != null)
            {
                task.Run();
            }
        }
    }
}
