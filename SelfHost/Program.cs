using System;
using Firefly.Http;

namespace SelfHost
{
	class Program
	{
		static void Main()
		{
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
