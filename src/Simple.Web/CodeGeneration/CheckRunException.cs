using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    using System.Threading.Tasks;
    using Http;

    internal static class CheckRunException
    {
        public static Task<Status> ImplAsync(Task<Status> task, IContext context)
        {
            if (!task.IsFaulted)
            {
                return task.ContinueWith(t => t.Result);
            }
            var tcs = new TaskCompletionSource<Status>();
            if (task.Exception != null && task.Exception.InnerExceptions.Any() && SimpleWeb.Configuration.ExceptionHandler != null)
            {
                tcs.SetResult(SimpleWeb.Configuration.ExceptionHandler.Handle(task.Exception.InnerExceptions.First(), context));
            }
            else
            {
                tcs.SetResult(new Status(500, "Internal server error"));
            }
            return tcs.Task;
        }

        public static Task<Status> ImplAsyncCheck(Exception exception, IContext context)
        {
            var status = SimpleWeb.Configuration.ExceptionHandler != null
                             ? SimpleWeb.Configuration.ExceptionHandler.Handle(exception, context)
                             : new Status(500, "Internal server error.");
            var tcs = new TaskCompletionSource<Status>();
            tcs.SetResult(status);
            return tcs.Task;
        }

        public static Status Impl(Exception exception, IContext context)
        {
            if (SimpleWeb.Configuration.ExceptionHandler != null)
            {
                return SimpleWeb.Configuration.ExceptionHandler.Handle(exception, context);
            }
            return new Status(500, "Internal server error.");
        }
    }
}
