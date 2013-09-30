namespace Simple.Web.Behaviors.Implementations
{
    using System;

    using Simple.Web.Helpers;
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
            if (context.Request.HttpMethod.Equals("HEAD"))
            {
                return;
            }

            if (!handler.Output.CanSeek)
            {
                throw new InvalidOperationException("Output stream must support Seek operations.");
            }
            context.Response.WriteFunction = stream =>
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