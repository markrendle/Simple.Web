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
            return task.ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        return t.Result;
                    }
                    if (t.Exception != null && t.Exception.InnerExceptions.Any() && SimpleWeb.Configuration.ExceptionHandler != null)
                    {
                        return SimpleWeb.Configuration.ExceptionHandler.Handle(t.Exception.InnerExceptions.First(), context);
                    }
                    else
                    {
                        return new Status(500, "Internal server error");
                    }
                });
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
