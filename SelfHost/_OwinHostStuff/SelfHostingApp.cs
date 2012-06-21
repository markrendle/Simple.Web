using System;
using System.Collections.Generic;
using Owin;
using Simple.Web;
using Simple.Web.Http;

namespace SelfHost
{
	public static class SelfHostingApp
	{


		public static void App(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
		{
			var wrapper = new ContextWrapper(env, result);

			AsyncCallback cb = CallCompleted(wrapper, result);
            
			var appTask = new Application().Run(wrapper).ToApm(cb,null);
			
			// TODO: fix this!
			var x = new Simple.Web.Razor.RazorHtmlMediaTypeHandler(); // to load the dll (temp)


			try {
				appTask.Wait();
			} catch (Exception ex){
				fault(ex);
			}
		}

		static AsyncCallback CallCompleted(IContext context, ResultDelegate result)
		{
			return ar => result(
				// Status:
			             	context.Response.StatusCode + " " +context.Response.StatusDescription,

			             	// Headers:
			             	new HeaderDictionary {
			             	                     	{"Content-Type", new[] {"text/html"}}
			             	                     }, 
				
			             	// Output
			             	(write,flush,end,cancel)=> {
			             		var response = (ResponseWrapper)context.Response;
			             		var bytes = response.Buffer.ToArray();
			             		write(new ArraySegment<byte>(bytes));
			             		end(null);
			             	});
		}
	}
}