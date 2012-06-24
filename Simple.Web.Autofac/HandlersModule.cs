namespace Simple.Web.Autofac
{
    using System.Reflection;
    using global::Autofac;

    class HandlersModule : global::Autofac.Module
    {
        private readonly Assembly _assembly;

        internal HandlersModule(Assembly assembly)
        {
            _assembly = assembly;                
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assembly)
                .Where(t =>
                    t.IsAssignableTo<IDelete>() ||
                    t.IsAssignableTo<IDeleteAsync>() ||
                    t.IsAssignableTo<IGet>() ||
                    t.IsAssignableTo<IGetAsync>() ||
                    t.IsAssignableTo<IHead>() ||
                    t.IsAssignableTo<IHeadAsync>() ||
                    t.IsAssignableTo<IPatch>() ||
                    t.IsAssignableTo<IPatchAsync>() ||
                    t.IsAssignableTo<IPost>() ||
                    t.IsAssignableTo<IPostAsync>() ||
                    t.IsAssignableTo<IPut>() ||
                    t.IsAssignableTo<IPutAsync>())
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
