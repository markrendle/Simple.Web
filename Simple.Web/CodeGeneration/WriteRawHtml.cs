namespace Simple.Web.CodeGeneration
{
    using System.Linq;

    static class WriteRawHtml
    {
        internal static void Impl(IOutput<RawHtml> endpoint, IContext context)
        {
            context.Response.ContentType =
                context.Request.AcceptTypes.FirstOrDefault(
                    at => at == ContentType.Html || at == ContentType.XHtml) ?? "text/html";
            context.Response.Output.Write(endpoint.Output.ToString());
        }
    }
}