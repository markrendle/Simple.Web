﻿namespace Simple.Web.Autofac
{
    using global::Autofac;

    public abstract class AutofacStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            configuration.Container = new AutofacContainer(BuildContainer());
        }

        protected internal abstract IContainer BuildContainer();
    }
}