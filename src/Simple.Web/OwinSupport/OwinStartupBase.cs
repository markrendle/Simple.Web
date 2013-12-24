namespace Simple.Web.OwinSupport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Owin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public abstract class OwinStartupBase
    {
        private readonly Action<IAppBuilder> _builder;

        protected Action<IAppBuilder> Builder
        {
            get
            {
                return this._builder;
            }
        }

        protected OwinStartupBase()
        {
            this._builder = builder => builder.UseSimpleWeb();
        }

        protected OwinStartupBase(Action<IAppBuilder> builder)
        {
            this._builder = builder;
        }

        public void Configuration(IAppBuilder app)
        {
            this._builder.Invoke(app);
        }
    }
}