namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class WriteStreamResponse
    {
        public static void Impl(IOutputStream outputStream, IContext context)
        {
            context.Response.ContentType = outputStream.ContentType;
            if (!string.IsNullOrWhiteSpace(outputStream.ContentDisposition))
            {
                context.Response.Headers.Set("Content-Disposition", outputStream.ContentDisposition);
            }
            if (context.Request.HttpMethod.Equals("HEAD")) return;

            using (var stream = outputStream.Output)
            {
                stream.Position = 0;
                stream.CopyTo(context.Response.OutputStream);
            }
        }
    }
}
