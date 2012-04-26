namespace Simple.Web
{
    using System.Collections.Generic;

    public interface ICookieCollection : ICollection<ICookie>
    {
        ICookie New(string name);
    }
}