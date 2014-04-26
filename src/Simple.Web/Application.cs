using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Simple.Web
{
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Behaviors.Implementations;
    using CodeGeneration;
    using Cors;
    using Helpers;
    using Hosting;
    using Http;

    using Routing;

    using Simple.Web.OwinSupport;
#pragma warning disable 811
    using Result = System.Tuple<System.Collections.Generic.IDictionary<string, object>, int, System.Collections.Generic.IDictionary<string, string[]>, System.Func<System.IO.Stream, System.Threading.Tasks.Task>>;
#pragma warning restore 811
    using AppFunc = Func<IDictionary<string,object>, Task>;

    /// <summary>
    /// The running application.
    /// </summary>
    public class Application
    {
        private static readonly object StartupLock = new object();
        private static volatile StartupTaskRunner _startupTaskRunner = new StartupTaskRunner();

        public static bool LegacyStaticContentSupport { get; set; }

        public static AppFunc App(AppFunc next)
        {
            return env => Run(env, next);
        }

        /// <summary>
        /// The OWIN standard application method.
        /// </summary>
        /// <param name="env"> Request life-time general variable storage.</param>
        /// <param name="next">The next app/component in the OWIN pipeline.</param>
        /// <returns>A <see cref="Task"/> which will complete the request.</returns>
        internal static Task Run(IDictionary<string, object> env, Func<IDictionary<string, object>, Task> next)
        {
            var context = new OwinContext(env);
            var task = Run(context);

            if (task == null)
            {
                return next(env);
            }
            
            return task
                .ContinueWith(t => WriteResponse(t, context, env)).Unwrap();
        }

        private static Func<Stream, Task> ErrorHandler(string message)
        {
            return stream =>
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                return stream.WriteAsync(bytes, 0, bytes.Length);
            };
        }

        private static Task WriteResponse(Task task, OwinContext context, IDictionary<string, object> env)
        {
            var tcs = new TaskCompletionSource<int>();

            var cancellationToken = (CancellationToken)env[OwinKeys.CallCancelled];

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetCanceled();
            }
            else if (task.IsFaulted || task.Exception != null)
            {
                context.Response.Status = Status.InternalServerError;
                context.Response.WriteFunction = ErrorHandler(task.Exception == null ? "An unknown error occured." : task.Exception.ToString());
            }
            else
            {
                try
                {
                    context.Response.EnsureContentTypeCharset();

                    env.Add(OwinKeys.StatusCode, context.Response.Status.Code);
                    env.Add(OwinKeys.ReasonPhrase, context.Response.Status.Description);

                    if (context.Response.Headers != null)
                    {
                        var responseHeaders = (IDictionary<string, string[]>)env[OwinKeys.ResponseHeaders];

                        foreach (var header in context.Response.Headers)
                        {
                            if (responseHeaders.ContainsKey(header.Key))
                            {
                                responseHeaders[header.Key] = header.Value;
                            }
                            else
                            {
                                responseHeaders.Add(header.Key, header.Value);
                            }
                        }
                    }

                    if (context.Response.WriteFunction != null)
                    {
                        return context.Response.WriteFunction((Stream)env[OwinKeys.ResponseBody]);
                    }

                    tcs.SetResult(0);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }

            return tcs.Task;
        }

        internal static Task Run(IContext context)
        {
            Startup();

            if (LegacyStaticContentSupport && StaticContent.TryHandleAsStaticContent(context))
            {
                return MakeCompletedTask();
            }

            IDictionary<string, string> variables;
            var handlerType = TableFor(context.Request.HttpMethod).Get(context.Request.Url.AbsolutePath, out variables, context.Request.GetContentType(), context.Request.GetAccept());
            if (handlerType == null) return null;
            var handlerInfo = new HandlerInfo(handlerType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.Keys.Where(k => !string.IsNullOrWhiteSpace(k)))
            {
                handlerInfo.Variables.Add(key, CombineQueryStringValues(context.Request.QueryString[key]));
            }

            var task = PipelineFunctionFactory.Get(handlerInfo.HandlerType, handlerInfo.HttpMethod)(context, handlerInfo);
            return task ?? MakeCompletedTask();
        }

        private static string CombineQueryStringValues(string[] values)
        {
            return values.Length == 1 ? values[0] : string.Join("\t", values);
        }

        private static Task MakeCompletedTask()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
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

        private static readonly ConcurrentDictionary<string, RoutingTable> RoutingTables = new ConcurrentDictionary<string, RoutingTable>(StringComparer.OrdinalIgnoreCase);

        internal static RoutingTable BuildRoutingTable(string httpMethod)
        {
            var types = ExportedTypeHelper.FromCurrentAppDomain(IsHttpMethodHandler).ToList();
            var handlerTypes = types
                .Where(i => HttpMethodAttribute.Matches(i, httpMethod))
                .ToArray();

            return new RoutingTableBuilder(handlerTypes).BuildRoutingTable();
        }

        private static bool IsHttpMethodHandler(Type type)
        {
            return (!type.IsInterface || type.IsAbstract) && HttpMethodAttribute.IsAppliedTo(type);
        }

        private static RoutingTable TableFor(string httpMethod)
        {
            return RoutingTables.GetOrAdd(httpMethod, BuildRoutingTable);
        }
    }
}