namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Simple.Web.Http;

    internal class PipelineBlock
    {
        private readonly List<MethodInfo> _methods = new List<MethodInfo>();
 
        public void Add(MethodInfo method)
        {
            _methods.Add(method);
        }

        public bool Any
        {
            get { return _methods.Count > 0; }
        }

        public bool IsBoolean
        {
            get { return _methods.Count > 0 && _methods.Last().ReturnType == typeof (bool); }
        }

        public Delegate Generate(Type handlerType)
        {
            var context = Expression.Parameter(typeof (IContext));
            var handler = Expression.Parameter(handlerType);

            var calls = new List<Expression>();
            calls.AddRange(_methods.Select(m => CreateCall(m, handler, context, handlerType)));

            if (_methods.Last().ReturnType == typeof(void))
            {
                calls.Add(Expression.Call(typeof(PipelineBlock).GetMethod("CompletedTask", BindingFlags.Static | BindingFlags.NonPublic)));
            }
            else if (_methods.Last().ReturnType == typeof(bool))
            {
                FixLastCall(calls, "CompleteBooleanTask");
            }
            else if (_methods.Last().ReturnType == typeof(Task))
            {
                FixLastCall(calls, "CompleteTask");
            }
            else if (_methods.Last().ReturnType == typeof(Task<bool>))
            {
                FixLastCall(calls, "CancelBooleanAsync");
            }
            else
            {
                throw new InvalidOperationException(
                    "Behavior implementation methods may only return void, bool, Task, or Task<bool>.");
            }

            var block = Expression.Block(calls);

            return Expression.Lambda(block, handler, context).Compile();
        }

        private static Expression CreateCall(MethodInfo method, ParameterExpression handler, ParameterExpression context, Type handlerType)
        {
            if (method.IsGenericMethod)
            {
                var handlerParameterType = method.GetParameters()[0].ParameterType;
                if (handlerParameterType.IsGenericType)
                {
                    var @interface =
                        handlerType.GetInterfaces().FirstOrDefault(
                            i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == handlerParameterType.GetGenericTypeDefinition());
                    if (@interface != null)
                    {
                        method = method.MakeGenericMethod(@interface.GetGenericArguments().Single());
                    }
                }
            }
            return Expression.Call(method, handler, context);
        }

        private static void FixLastCall(List<Expression> calls, string methodName)
        {
            var lastCall = calls.Last();
            calls.Remove(lastCall);
            calls.Add(Expression.Call(typeof(PipelineBlock).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic), lastCall));
        }

        private static Task<bool> CompletedTask()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            return tcs.Task;
        }

        private static Task<bool> CompleteBooleanTask(bool @continue)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (@continue)
            {
                tcs.SetResult(true);
            }
            else
            {
                tcs.SetCanceled();
            }
            return tcs.Task;
        }

        private static Task<bool> CompleteTask(Task task)
        {
            return task.ContinueWith(t => true, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private static Task<bool> CancelBooleanAsync(Task<bool> task)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return task.ContinueWith(t =>
                {
                    if (!t.Result)
                    {
                        cancellationTokenSource.Cancel();
                    }
                    return t.Result;
                }, cancellationTokenSource.Token);
        }
    }
}