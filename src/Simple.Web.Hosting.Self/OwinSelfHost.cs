namespace Simple.Web.Hosting.Self
{
    using System;

    using Microsoft.Owin.Hosting;

    using Owin;

    using Simple.Web.OwinSupport;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class OwinSelfHost : OwinStartupBase
    {
        public const string DefaultHostname = "localhost";

        public const int DefaultPort = 3333;

        public OwinSelfHost()
            : base(builder =>
                   {
                       builder.UseErrorPage();
                       builder.UseSimpleWeb();
                   })
        {
        }

        public OwinSelfHost(Action<IAppBuilder> builder)
            : base(builder)
        {
        }

        public void Run(string hostname = DefaultHostname, int port = DefaultPort, bool ssl = false)
        {
            using (Start(hostname, port, ssl))
            {
                Console.WriteLine("Listening at http{0}://{1}:{2}. Press CTRL-C to stop.", ssl ? "s" : string.Empty, hostname, port);

                ConsoleKeyInfo cki;

                do
                {
                    cki = Console.ReadKey();
                }
                while (!(((cki.Modifiers & ConsoleModifiers.Control) != 0) && cki.Key == ConsoleKey.C));
            }
        }

        public IDisposable Start(string hostname = DefaultHostname, int port = DefaultPort, bool ssl = false)
        {
            var startOptions = new StartOptions(string.Format("http{2}://{0}:{1}", hostname, port, ssl ? "s" : string.Empty));

            return WebApp.Start(startOptions, Builder);
        }
    }
}