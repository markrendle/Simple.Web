namespace Simple.Web.Behaviors.Implementations
{
    using Helpers;
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
        public static void Impl(IOutputStream handler, IContext context)
        {
            context.Response.SetContentType(handler.ContentType);
            if (!string.IsNullOrWhiteSpace(handler.ContentDisposition))
            {
                context.Response.SetHeader("Content-Disposition", handler.ContentDisposition);
            }
            if (context.Request.HttpMethod.Equals("HEAD")) return;

            context.Response.WriteFunction = (stream, token) =>
                {
                    using (var outputStream = handler.Output)
                    {
                        outputStream.Position = 0;
                        outputStream.CopyTo(stream);
                    }
                    return TaskHelper.Completed();
                };
        }
    }
}
