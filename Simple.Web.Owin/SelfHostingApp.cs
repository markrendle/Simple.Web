using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	public static class SelfHostingApp
	{
		public static void App(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault) {
			var wrapper = new ContextWrapper(env);

			var appTask = new Application()
				.Run(wrapper)
				.ToApm(CallCompleted(wrapper, result), null);

			try {
				appTask.Wait();
				if (appTask.Exception != null) throw appTask.Exception;
			} catch (Exception) {
				OwinOutput.SendFailureResult(wrapper, result, (Task<object>)appTask);
			}
		}

		static AsyncCallback CallCompleted(IContext context, ResultDelegate result) {
			return ar => {
				var response = (ResponseWrapper)context.Response;
				var task = ar as Task<object>;
				if (task != null && (task.Exception != null || task.IsFaulted))
				{
					OwinOutput.SendFailureResult(context, result, task);
				}
				else
				{
					OwinOutput.SendSuccessResult(result, response);
				}
			};
		}

		/// <summary>
		/// Catch all otherwise missed exceptions.
		/// Failing to do this will cause the host to die.
		/// This should get handled better!
		/// </summary>
		static SelfHostingApp() {
			TaskScheduler.UnobservedTaskException += (sender, args) => {
					Console.WriteLine(args.Exception.Message);
					args.SetObserved();
				};
		}

		public static void Use<T>() where T : new()
		{
			new T();
		}
	}
}