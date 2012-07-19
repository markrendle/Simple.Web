#if OWIN
using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	class OwinOutput
	{
		public static void SendSuccessResult(ResultDelegate result, ResponseWrapper response)
		{
			result(
				// Status:
				response.StatusCode + " " + response.StatusDescription,

				// Headers:
				response.Headers.Set("Content-Type",response.ContentType ?? ""),

				// Output
				(write, flush, end, cancel) => {
					var bytes = response.Buffer.ToArray();
					if (bytes.LongLength > 0)
					{
						write(new ArraySegment<byte>(bytes));
					}
					end(null);
				});
		}

		#region Failure case
		public static void SendFailureResult(IContext context, ResultDelegate result, Task<object> task)
		{
			var ctx = context as ContextWrapper;
			if (AccessedByNonLocalUrl(ctx))
			{
				WritePublicSafeError(result);
			} else
			{
				WriteDetailedError(ctx, result, task);
			}
		}

		static bool AccessedByNonLocalUrl(ContextWrapper ctx)
		{
			return ctx == null || (ctx.Request.Url.Host != "localhost" && ctx.Request.Url.Host != "127.0.0.1");
		}

		static void WriteDetailedError(ContextWrapper ctx, ResultDelegate result, Task<object> task)
		{
			result(
				"500 InternalError",
				new HeaderDictionary
				{
					{"Content-Type", new[] {"text/plain"}}
				},
				(write, flush, end, cancel) => {
					var sb = new StringBuilder();
					sb.AppendLine("Detailed error report (request came from localhost) at "+DateTime.Now);
					sb.AppendLine(ctx.Request.Url.ToString());
					sb.Append("Faulted -- ");
					BuildExceptionTrace(task.Exception, sb);
					sb.AppendLine("Headers:");
					Stringify(ctx.Request.Headers, sb);

					var bytes = Encoding.Default.GetBytes(sb.ToString());
					write(new ArraySegment<byte>(bytes));
					end(null);
				}
				);
		}

		static void Stringify(NameValueCollection headers, StringBuilder sb)
		{
			foreach (var key in headers.AllKeys)
			{
				sb.AppendLine(key + ": "+ headers[key]);
			}
		}

		static void BuildExceptionTrace(Exception ex, StringBuilder sb)
		{
			if (ex == null) {
				sb.AppendLine("No exception!");
				return;
			}
			sb.AppendLine(ex.Message);
			sb.AppendLine(ex.StackTrace);
			sb.AppendLine("-------------------------");
			if (ex.InnerException != null) BuildExceptionTrace(ex.InnerException, sb);
		}

		static void WritePublicSafeError(ResultDelegate result)
		{
			result(
				"500 InternalError",
				new HeaderDictionary
				{
					{"Content-Type", new[] {"text/plain"}}
				},
				(write, flush, end, cancel) => {
					var bytes = Encoding.Default.GetBytes("Sorry, something went wrong trying to show you this page.\r\n(If you are trying to fix it, view the page from http://localhost/)");
					write(new ArraySegment<byte>(bytes));
					end(null);
				}
				);
		}
		#endregion
	}
}
#endif