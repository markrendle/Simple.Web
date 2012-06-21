using System;
using System.Threading;
using System.Threading.Tasks;

namespace SelfHost
{
	internal static class TaskEx
	{
		/// <summary>
		/// A Task extension method that converts this object to an IAsyncResult.
		/// </summary>
		/// <remarks>
		/// Mark, 19/06/2012.
		/// Props to Stephen Toub for this blog post:
		/// http://blogs.msdn.com/b/pfxteam/archive/2011/06/27/10179452.aspx
		/// </remarks>
		/// <param name="task"> The task to act on.</param>
		/// <param name="callback">The callback.</param>
		/// <param name="state">The state.</param>
		/// <returns>
		/// The given data converted to an IAsyncResult-y Task.
		/// </returns>
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