namespace Autofac
{
    using Simple.Web.Autofac;
    using System.Reflection;

    public static class RegisterHandlersExtension
    {
        public static void RegisterHandlersInAssembly(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterModule(new HandlersModule(assembly));
        }
    }
}
