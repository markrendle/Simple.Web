using System;
using Firefly.Http;
using Simple.Web.Owin;

namespace SelfHost
{
	class Program
	{
		static void Main()
		{
			// TODO: fix this!
			new Simple.Web.Razor.RazorHtmlMediaTypeHandler(); // Force load the Razor dll.


			var server = new ServerFactory();
			using (server.Create(SelfHostingApp.App, port: 81))
			{
				Console.WriteLine("Running server on http://localhost:81/");
				Console.WriteLine("Press enter to exit");
				Console.ReadLine();
			}
		}
	}
}
