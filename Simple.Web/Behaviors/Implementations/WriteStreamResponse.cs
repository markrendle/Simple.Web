namespace Simple.Web.Behaviors.Implementations
{
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class WriteStreamResponse
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(IOutputStream outputStream, IContext context)
        {
            context.Response.ContentType = outputStream.ContentType;
            if (!string.IsNullOrWhiteSpace(outputStream.ContentDisposition))
            {
                context.Response.SetHeader("Content-Disposition", outputStream.ContentDisposition);
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
