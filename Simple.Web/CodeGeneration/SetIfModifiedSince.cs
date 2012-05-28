namespace Simple.Web.CodeGeneration
{
    using System;

    internal static class SetIfModifiedSince
    {
        internal static void Impl(IModified handler, IContext context)
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