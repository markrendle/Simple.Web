using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firefly.Http;
using Simple.Web;
using Owin;

namespace SelfHost
{
	class Program
	{
		static void Main()
		{
			var server = new ServerFactory();
			using (server.Create(App, 81))
			{
				Console.WriteLine("Running server on http://localhost:81/");
				Console.WriteLine("Press enter to exit");
				Console.ReadLine();
			}
		}

		private static void App(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
		{
			var wrapper = new ContextWrapper(env, result);

			AsyncCallback cb = CallCompleted(wrapper, result);
            
			var appTask = new Application().Run(wrapper).ToApm(cb,null);
			var x = new Simple.Web.Razor.RazorHtmlMediaTypeHandler(); // to load the dll (temp)

			try {
				appTask.Wait();
			} catch (Exception ex){
				fault(ex);
			}
		}

		static AsyncCallback CallCompleted(ContextWrapper context, ResultDelegate result)
		{
			return ar => result(
				context.Response.StatusCode + " " +context.Response.StatusDescription,
				new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
				{
					{"Content-Type", new[] {"text/plain"}}
				}, (write,flush,end,cancel)=> {
					var response = (ResponseWrapper)context.Response;
					var bytes = response.Buffer.ToArray();
					write(new ArraySegment<byte>(bytes));
					end(null);
				});
		}
	}

	
    internal static class TaskEx
    {
        /// <summary>
        /// A Task extension method that converts this object to an IAsyncResult.
        /// </summary>
        /// <remarks>
        /// Mark, 19/06/2012.
        /// Props to Stephen Toub for this blog post:
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/06/27/10179452.aspx
        /// </remarks>
        /// <param name="task"> The task to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// The given data converted to an IAsyncResult-y Task.
        /// </returns>
        public static Task ToApm(this Task task, AsyncCallback callback, object state)
        {
            task = task ?? MakeCompletedTask();
            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    }
                    else if (t.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }

                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return tcs.Task;
        }

        private static Task MakeCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}
