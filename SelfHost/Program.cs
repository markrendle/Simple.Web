using System;
using System.Collections.Generic;
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
			
            var appTask = new Application().Run(new ContextWrapper(env, result));

			try {
				appTask.RunSynchronously(); // crap for now!
			} catch (Exception ex){fault(ex);}

			// Return a 200 and end
			result(
				 "200 OK",
				 new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
                {
                    {"Content-Type", new[] {"text/plain"}}
                }, (a,b,end,d)=> end(null));
		}
	}
}
