namespace Autofac
{
    using System.Reflection;

    using Simple.Web.Autofac;

    public static class RegisterHandlersExtension
    {
        public static void RegisterHandlersInAssembly(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterModule(new HandlersModule(assembly));
        }
    }
}