namespace Simple.Web
{
    using System.Collections.Generic;

    public interface ISetCookies
    {
        IEnumerable<ICookie> CookiesToSet { get; }
    }
}