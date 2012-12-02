using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Web.Autofac
{
    using System.Reflection;
    using global::Autofac;

    class HandlersModule: global::Autofac.Module
    {
        private readonly Assembly _assembly;
        private readonly List<Func<Type, Type>> _typeDictionary = LoadTypeDictionary();

        internal HandlersModule(Assembly assembly)
        {
            _assembly = assembly;                
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assembly)
                .Where( t => t.IsClass && !t.IsAbstract && 
                    _typeDictionary.Any(l => l.Invoke(t) != null))
                .AsSelf()
                .InstancePerLifetimeScope();
        }

        private static List<Func<Type, Type>> LoadTypeDictionary()
        {
            return new List<Func<Type, Type>>
                {  
                     x => x.GetInterface(typeof (IDelete).Name)
                    ,x => x.GetInterface(typeof (IDeleteAsync).Name)
                    ,x => x.GetInterface(typeof (IGet).Name)
                    ,x => x.GetInterface(typeof (IGetAsync).Name)
                    ,x => x.GetInterface(typeof (IHead).Name)
                    ,x => x.GetInterface(typeof (IHeadAsync).Name)
                    ,x => x.GetInterface(typeof (IPatch).Name)
                    ,x => x.GetInterface(typeof (IPatchAsync).Name)
                    ,x => x.GetInterface(typeof (IPatch<>).Name)
                    ,x => x.GetInterface(typeof (IPatchAsync<>).Name)
                    ,x => x.GetInterface(typeof (IPostAsync).Name)
                    ,x => x.GetInterface(typeof (IPost<>).Name)
                    ,x => x.GetInterface(typeof (IPostAsync<>).Name)
                    ,x => x.GetInterface(typeof (IPut).Name) 
                    ,x => x.GetInterface(typeof (IPutAsync).Name) 
                    ,x => x.GetInterface(typeof (IPut<>).Name) 
                    ,x => x.GetInterface(typeof (IPutAsync<>).Name) 
                };
        }
    }
}
