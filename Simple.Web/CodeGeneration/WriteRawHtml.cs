namespace Simple.Web.CodeGeneration
{
    using System.Linq;
    using Behaviors;
    using ContentTypeHandling;
    using Http;

    static class WriteRawHtml
    {
        internal static void Impl(IOutput<RawHtml> handler, IContext context)
        {
            context.Response.ContentType =
                context.Request.AcceptTypes.FirstOrDefault(
                    at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
            if (context.Request.HttpMethod.Equals("HEAD")) return;
            context.Response.Output.Write(handler.Output.ToString());
        }
    }
}