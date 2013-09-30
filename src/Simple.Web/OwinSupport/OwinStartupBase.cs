namespace Simple.Web.OwinSupport
{
    using System;

    using Owin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public abstract class OwinStartupBase
    {
        private readonly Action<IAppBuilder> _builder;

        protected OwinStartupBase()
        {
            _builder = builder => builder.Use(new Func<AppFunc, AppFunc>(ignoreNextApp => (AppFunc)Application.Run));
        }

        protected OwinStartupBase(Action<IAppBuilder> builder)
        {
            _builder = builder;
        }

        protected Action<IAppBuilder> Builder
        {
            get { return _builder; }
        }

        public void Configuration(IAppBuilder app)
        {
            _builder.Invoke(app);
        }
    }
}