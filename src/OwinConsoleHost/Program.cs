namespace OwinConsoleHost
{
    using System;

    using Owin;

    using Simple.Web.Hosting.Self;
    using Simple.Web.OwinSupport;
    using Simple.Web.Razor;

    internal class Program
    {
        private static Type razor = typeof(AmbiguousViewException);

        private static void AdvancedHost()
        {
            new OwinSelfHost(builder =>
                             {
                                 builder.UseErrorPage();
                                 builder.UseSimpleWeb();
                             }).Run();
        }

        private static void DisposableHost()
        {
            var host = new OwinSelfHost();

            using (host.Start(hostname: "localhost", port: 3333, ssl: false))
            {
                Console.WriteLine("Listening at http://localhost:3333. Press CTRL-C to stop.");

                Console.CancelKeyPress += (sender, eventArgs) => Console.WriteLine("Stopping.");

                while (true)
                {
                }
            }
        }

        private static void Main(string[] args)
        {
            SimpleHost();
        }

        private static void SimpleHost()
        {
            new OwinSelfHost().Run(hostname: "localhost", port: 3333, ssl: false);
        }
    }
}