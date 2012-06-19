namespace Simple.Web.AspNet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Authentication;
    using Hosting;
    using Http;

    internal class SimpleHttpAsyncHandler : IHttpAsyncHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var app = new Application();
            var appTask = app.Run(new ContextWrapper(context));
            return appTask.ToApm(cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }
    }

    internal class TaskAsyncResult : IAsyncResult
    {
        public TaskAsyncResult(Task task, AsyncCallback callback)
        {
            if (task != null)
            {
                task.ContinueWith(t => callback(this));
            }
        }

        public bool IsCompleted { get; private set; }
        public WaitHandle AsyncWaitHandle { get; private set; }
        public object AsyncState { get; private set; }
        public bool CompletedSynchronously { get; private set; }
    }

    internal static class TaskEx
    {
        public static Task ToApm(this Task task, AsyncCallback callback, object state)
        {
            task = task ?? MakeCompletedTask();
            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    }
                    else if (t.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }

                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return tcs.Task;
        }

        private static Task MakeCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}