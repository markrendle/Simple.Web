using System;
using System.Collections.Generic;
using Owin;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	public static class SelfHostingApp
	{


		public static void App(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
		{
			var wrapper = new ContextWrapper(env, result);

			AsyncCallback cb = CallCompleted(wrapper, result);
            
			var appTask = new Application().Run(wrapper).ToApm(cb,null);

			try {
				appTask.Wait();
			} catch (Exception ex){
				fault(ex);
			}
		}

		static AsyncCallback CallCompleted(IContext context, ResultDelegate result) {
			return ar => {
				var response = (ResponseWrapper)context.Response;
				result(
				// Status:
				response.StatusCode + " " + response.StatusDescription,

				// Headers:
				new HeaderDictionary {
									  {"Content-Type", new[] {response.ContentType ?? ""}}
								   },

				// Output
				(write, flush, end, cancel) => {
					var bytes = response.Buffer.ToArray();
					if (bytes.LongLength > 0) {
						write(new ArraySegment<byte>(bytes));
					}
					end(null);
				});
			};
		}
	}
}