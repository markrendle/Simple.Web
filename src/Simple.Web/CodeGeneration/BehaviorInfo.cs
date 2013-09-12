namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Behaviors;
    using Helpers;
    using Http;

    abstract class BehaviorInfo
    {
        private readonly Type _behaviorType;
        private readonly Type _implementingType;
        private readonly Priority _priority;
        
        protected BehaviorInfo(Type behaviorType, Type implementingType, Priority priority)
        {
            _behaviorType = behaviorType;
            _implementingType = implementingType;
            _priority = priority;
        }

        public Priority Priority
        {
            get { return _priority; }
        }

        public Type ImplementingType
        {
            get { return _implementingType; }
        }

        public Type BehaviorType
        {
            get { return _behaviorType; }
        }

        public bool Universal { get; set; }

        public MethodInfo GetMethod()
        {
            foreach (var methodInfo in ImplementingType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 2 && parameters[1].ParameterType == typeof(IContext))
                {
                    if (parameters[0].ParameterType == BehaviorType)
                    {
                        return methodInfo;
                    }

                    if (parameters[0].ParameterType.IsGenericType && BehaviorType.IsGenericType)
                    {
                        if (parameters[0].ParameterType.GetGenericTypeDefinition() == BehaviorType.GetGenericTypeDefinition())
                        {
                            return methodInfo;
                        }
                    }
                }
            }

            throw new MissingMethodException(ImplementingType.Name, "Implementation");
        }

        public MethodInfo GetMethod(Type[] genericTypes)
        {
            foreach (var methodInfo in ImplementingType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 2) continue;
                if (parameters[1].ParameterType != typeof(IContext)) continue;

                var genericArguments = methodInfo.GetGenericArguments();
                if (genericArguments.Length != genericTypes.Length) continue;

                return methodInfo.MakeGenericMethod(genericTypes);
            }

            throw new MissingMethodException(ImplementingType.Name, "Implementation");
        }

        protected static IEnumerable<TInfo> FindBehaviorTypes<TAttribute,TInfo>(Func<Type,Type,Priority,TInfo> construct)
            where TAttribute : BehaviorAttribute
        {
			foreach (var behaviorType in ExportedTypeHelper.FromCurrentAppDomain(type => IsBehaviorType<TAttribute>(type)))
            {
                var attribute = (TAttribute) Attribute.GetCustomAttribute(behaviorType, typeof(TAttribute));
                if (attribute != null)
                {
                    yield return construct(behaviorType, attribute.ImplementingType, attribute.Priority);
                }
            }
        }

        private static bool IsBehaviorType<T>(Type type)
            where T : BehaviorAttribute
        {
            return Attribute.GetCustomAttribute(type, typeof(T)) != null;
        }
    }
}