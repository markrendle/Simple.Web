namespace Simple.Web.CodeGeneration
{
    static class WriteStreamResponse
    {
        public static void Impl(IOutputStream outputStream, IContext context)
        {
            context.Response.ContentType = outputStream.ContentType;
            if (!string.IsNullOrWhiteSpace(outputStream.ContentDisposition))
            {
                context.Response.Headers.Set("Content-Disposition", outputStream.ContentDisposition);
            }
            using (var stream = outputStream.Output)
            {
                stream.Position = 0;
                stream.CopyTo(context.Response.OutputStream);
            }
        }
    }
}
