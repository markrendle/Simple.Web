namespace Simple.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using CodeGeneration;
    using Helpers;
    using Hosting;
    using Http;
    using Routing;

    /// <summary>
    /// The running application.
    /// </summary>
    public class Application
    {
        private static readonly object StartupLock = new object();
        private static StartupTaskRunner _startupTaskRunner = new StartupTaskRunner();

        public Task Run(IContext context)
        {
            Startup();
            IDictionary<string, string> variables;
            var handlerType = TableFor(context.Request.HttpMethod).Get(context.Request.Url.AbsolutePath, context.Request.ContentType, context.Request.AcceptTypes, out variables);
            if (handlerType == null) return null;
            var handlerInfo = new HandlerInfo(handlerType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.Select(g => g.Key))
            {
                handlerInfo.Variables.Add(key, context.Request.QueryString[key].FirstOrDefault());
            }

            if (handlerInfo.IsAsync)
            {
                var handler = HandlerFactory.Instance.GetHandler(handlerInfo);

                if (handler != null)
                {
                    var runner = HandlerRunnerFactory.Instance.GetAsync(handlerInfo.HandlerType, context.Request.HttpMethod);
                    return runner.Start(handler.Handler, context).ContinueWith(t => RunContinuation(t, handler, context, runner));
                }
                throw new InvalidOperationException("Could not create handler.");
            }
            else
            {
                var tcs = new TaskCompletionSource<object>();
                try
                {
                    using (var handler = HandlerFactory.Instance.GetHandler(handlerInfo))
                    {
                        if (handler == null)
                        {
                            throw new InvalidOperationException("Could not create handler.");
                        }
                        var run = HandlerRunnerFactory.Instance.Get(handler.Handler.GetType(),
                                                                    context.Request.HttpMethod);
                        run(handler.Handler, context);
                    }
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                return tcs.Task;
            }
        }

        private static void Startup()
        {
            if (_startupTaskRunner != null)
            {
                lock (StartupLock)
                {
                    if (_startupTaskRunner != null)
                    {
                        _startupTaskRunner.RunStartupTasks();
                        _startupTaskRunner = null;
                    }
                }
            }
        }

        private void RunContinuation(Task<Status> t, IScopedHandler handler, IContext context, AsyncRunner runner)
        {
            try
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    new ErrorHelper(context).WriteError(t.Exception.InnerException);
                }
                else
                {
                    try
                    {
                        runner.End(handler.Handler, context, t.Result);
                    }
                    catch (Exception ex)
                    {
                        new ErrorHelper(context).WriteError(ex);
                    }
                }
            }
            finally
            {
                handler.Dispose();
            }
        }

        private static readonly ConcurrentDictionary<string, RoutingTable> RoutingTables = new ConcurrentDictionary<string, RoutingTable>(StringComparer.OrdinalIgnoreCase);

        private static RoutingTable BuildRoutingTable(string httpMethod)
        {
            var handlerTypes = ExportedTypeHelper.FromCurrentAppDomain(IsHttpMethodHandler)
                .Where(i => HttpMethodAttribute.Get(i).HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return new RoutingTableBuilder(handlerTypes).BuildRoutingTable();
        }

        private static bool IsHttpMethodHandler(Type type)
        {
            return HttpMethodAttribute.IsAppliedTo(type);
        }

        private static RoutingTable TableFor(string httpMethod)
        {
            return RoutingTables.GetOrAdd(httpMethod, BuildRoutingTable);
        }
    }
}