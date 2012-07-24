namespace Simple.Web.Helpers
{
    using System;
    using System.Threading.Tasks;

    public class TaskHelper
    {
        public static Task<T> Completed<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }
        
        public static Task Completed()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        public static Task Exception(Exception exception)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}