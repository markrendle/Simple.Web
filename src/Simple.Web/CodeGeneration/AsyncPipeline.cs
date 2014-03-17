namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using Helpers;
    using Simple.Web;
    using Simple.Web.DependencyInjection;
    using Simple.Web.Http;

    internal class AsyncPipeline
    {
        public static readonly MethodInfo DefaultStartMethod = typeof (AsyncPipeline).GetMethod("DefaultStart",
                                                                                                BindingFlags.Static |
                                                                                                BindingFlags.NonPublic);
        public static MethodInfo StartMethod(Type handlerType)
        {
            return GetMethod("Start", handlerType);
        }

        public static MethodInfo ContinueWithHandlerMethod(Type handlerType)
        {
            return GetMethod("ContinueWithHandler", handlerType);
        }

        public static MethodInfo ContinueWithAsyncHandlerMethod(Type handlerType)
        {
            return GetMethod("ContinueWithAsyncHandler", handlerType);
        }

        public static MethodInfo ContinueWithAsyncBlockMethod(Type handlerType)
        {
            return GetMethod("ContinueWithAsyncBlock", handlerType);
        }

        public static MethodInfo ContinueWithActionMethod(Type handlerType)
        {
            return GetMethod("ContinueWithAction", handlerType);
        }

        private static MethodInfo GetMethod(string name, Type handlerType)
        {
            return typeof (AsyncPipeline).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(handlerType);
        }

        private static Task<bool> DefaultStart()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            return tcs.Task;
        }

        private static Task<bool> Start<THandler>(Func<THandler, IContext, Task<bool>> func, IContext context, THandler handler)
        {
            return func(handler, context);
        }

        private static Task<bool> ContinueWithAsyncBlock<THandler>(Task<bool> task, Func<THandler, IContext, ISimpleContainerScope, Task<bool>> continuation, IContext context, THandler handler, ISimpleContainerScope container)
        {
            return task.ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        return continuation(handler, context, container);
                    }
                    return TaskHelper.Completed(false);
                }, TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();
        }

        private static Task<bool> ContinueWithHandler<THandler>(Task<bool> task, Func<THandler, IContext, ISimpleContainerScope, Status> continuation, IContext context, THandler handler, ISimpleContainerScope container)
        {
            return task.ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        context.Response.Status = continuation(handler, context, container);
                        return true;
                    }
                    return false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private static Task<bool> ContinueWithAction<THandler>(Task<bool> task, Action<THandler, IContext, ISimpleContainerScope> continuation, IContext context, THandler handler, ISimpleContainerScope container)
        {
            return task.ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        continuation(handler, context, container);
                        return true;
                    }
                    return false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private static Task<bool> ContinueWithAsyncHandler<THandler>(Task<bool> task, Func<THandler, IContext, ISimpleContainerScope, Task<Status>> continuation, IContext context, THandler handler, ISimpleContainerScope container)
        {
            return task.ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        return continuation(handler, context, container)
                            .ContinueWith(ht =>
                                              {
                                                  context.Response.Status = ht.Result;
                                                  return true;
                                              });
                    }
                    return TaskHelper.Completed(false);
                }, TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();
        }
        
       
    }
}
