namespace Simple.Web.CodeGeneration
{
    using System.Linq;
    using System.Text;
    using Behaviors;
    using Helpers;
    using Http;
    using MediaTypeHandling;

    static class WriteRawHtml
    {
        internal static void Impl(IOutput<RawHtml> handler, IContext context)
        {
            context.Response.SetContentType(
                context.Request.GetAccept().FirstOrDefault(
                    at => at == MediaType.Html || at == MediaType.XHtml) ?? "text/html");
            if (context.Request.HttpMethod.Equals("HEAD")) return;

            context.Response.WriteFunction = (stream) =>
                {
                    var bytes = Encoding.UTF8.GetBytes(handler.Output.ToString());
                    return stream.WriteAsync(bytes, 0, bytes.Length);
                };
        }
    }
}