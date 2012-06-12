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
            return new TaskAsyncResult(app.Run(new ContextWrapper(context)), cb);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }
    }

    internal class TaskAsyncResult : AsyncResult
    {
        public TaskAsyncResult(Task task, AsyncCallback callback)
        {
            task.ContinueWith(t => callback(this));
        }
    }
}