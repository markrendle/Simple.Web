namespace Simple.Web.Behaviors.Implementations
{
    using System;
    using Simple.Web.Behaviors;
    using Simple.Web.Http;

    public static class SetIfModifiedSince
    {
        public static void Impl(IModified handler, IContext context)
        {
            var header = context.Request.Headers["If-Modified-Since"];
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