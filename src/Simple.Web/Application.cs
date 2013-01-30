﻿namespace Simple.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Behaviors.Implementations;
    using CodeGeneration;
    using Helpers;
    using Hosting;
    using Http;
    using Owin;
    using Routing;
#pragma warning disable 811
    using Result = System.Tuple<System.Collections.Generic.IDictionary<string, object>, int, System.Collections.Generic.IDictionary<string, string[]>, System.Func<System.IO.Stream, System.Threading.Tasks.Task>>;
#pragma warning restore 811

    /// <summary>
    /// The running application.
    /// </summary>
    public class Application
    {
        private static readonly object StartupLock = new object();
        private static volatile StartupTaskRunner _startupTaskRunner = new StartupTaskRunner();

        /// <summary>
        /// The OWIN standard application method.
        /// </summary>
        /// <param name="env"> Request life-time general variable storage </param>
        /// <returns>A <see cref="Task"/> which will complete the request.</returns>
        [Export("Owin.Application")]
        public static Task Run(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);
            var task = Run(context);
            if (task == null)
            {
                return TaskHelper.Completed(new Result(null, 404, null, null));
            }
            return task
                .ContinueWith(t => WriteResponse(context, env)).Unwrap();
        }

        private static Task WriteResponse(OwinContext context, IDictionary<string, object> env)
        {
            var tcs = new TaskCompletionSource<int>();
            var cancellationToken = (CancellationToken) env[OwinKeys.CallCancelled];
            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetCanceled();
            }
            else
            {
                try
                {
                    env[OwinKeys.StatusCode] = context.Response.Status.Code;
                    env[OwinKeys.ReasonPhrase] = context.Response.Status.Description;
                    env[OwinKeys.ResponseHeaders] = context.Response.Headers;
                    if (context.Response.WriteFunction != null)
                    {
                        return context.Response.WriteFunction((Stream) env[OwinKeys.ResponseBody]);
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

            if (TryHandleAsStaticContent(context))
            {
                return MakeCompletedTask();
            }

            IDictionary<string, string[]> variables;
            var handlerType = TableFor(context.Request.HttpMethod).Get(context.Request.Url.AbsolutePath, context.Request.GetContentType(), context.Request.GetAccept(), out variables);
            if (handlerType == null) return null;
            var handlerInfo = new HandlerInfo(handlerType, variables, context.Request.HttpMethod);

            foreach (var key in context.Request.QueryString.Keys.Where(k => !string.IsNullOrWhiteSpace(k)))
            {
                handlerInfo.Variables.Add(key, context.Request.QueryString[key]);
            }

            var task = PipelineFunctionFactory.Get(handlerInfo.HandlerType, handlerInfo.HttpMethod)(context, handlerInfo);
            return task ?? MakeCompletedTask();
        }

        private static Task MakeCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        private static bool TryHandleAsStaticContent(IContext context)
        {
            var absolutePath = context.Request.Url.AbsolutePath;
            string file;
            if (SimpleWeb.Configuration.PublicFileMappings.ContainsKey(absolutePath))
            {
                file = SimpleWeb.Environment.PathUtility.MapPath(SimpleWeb.Configuration.PublicFileMappings[absolutePath]);
            }
            else if (SimpleWeb.Configuration.AuthenticatedFileMappings.ContainsKey(absolutePath))
            {
                var user = SimpleWeb.Configuration.AuthenticationProvider.GetLoggedInUser(context);
                if (user == null || !user.IsAuthenticated)
                {
                    CheckAuthentication.Redirect(context);
                    return true;
                }
                file = SimpleWeb.Environment.PathUtility.MapPath(SimpleWeb.Configuration.AuthenticatedFileMappings[absolutePath]);
            }
            else if (
                SimpleWeb.Configuration.PublicFolders.Any(
                    folder => absolutePath.StartsWith(folder + "/", StringComparison.OrdinalIgnoreCase)))
            {
                file = SimpleWeb.Environment.PathUtility.MapPath(absolutePath);
            }
            else
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return false;

            context.Response.Status = Status.OK;
            context.Response.SetContentType(GetContentType(file, context.Request.GetAccept()));
            context.Response.SetContentLength(new FileInfo(file).Length);
            context.Response.WriteFunction = (stream) =>
                {
                    using (var fileStream = File.OpenRead(file))
                    {
                        fileStream.CopyTo(stream);
                        return TaskHelper.Completed();
                    }
                };

            return true;
        }

        internal static string GetContentType(string file, IEnumerable<string> acceptTypes)
        {
            if (acceptTypes == null) return "text/plain";

			var types = acceptTypes.ToArray();

			if (types.All(r=>r == "*/*")) return GuessType(file);
            return types.FirstOrDefault() ?? "text/plain";
        }

    	static string GuessType(string file)
    	{
    		switch (file.ToLower().SubstringAfterLast('.'))
    		{
				case "js":
				case "javascript": return "text/javascript";

				case "css": return "text/css";

				case "jpg":
				case "jpeg": return "image/jpeg";
				case "png": return "image/png";
				case "gif": return "image/gif";

                case "html":
                case "htm":
                case "xhtml": return "text/html";

				default: return "text/plain";
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

        private static readonly ConcurrentDictionary<string, RoutingTable> RoutingTables = new ConcurrentDictionary<string, RoutingTable>(StringComparer.OrdinalIgnoreCase);

        private static RoutingTable BuildRoutingTable(string httpMethod)
        {
            var types = ExportedTypeHelper.FromCurrentAppDomain(IsHttpMethodHandler).ToList();
            var handlerTypes = types
                .Where(i => HttpMethodAttribute.Matches(i,httpMethod))
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