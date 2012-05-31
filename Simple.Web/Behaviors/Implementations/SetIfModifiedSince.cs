namespace Simple.Web.Behaviors.Implementations
{
    using System;
    using System.Linq;
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetIfModifiedSince
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(IModified handler, IContext context)
        {
            var header = context.Request.Headers["If-Modified-Since"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(header))
            {
                DateTime time;
                if (DateTime.TryParse(header, out time))
                {
                    handler.IfModifiedSince = time;
                }
            }
        }
    }
}