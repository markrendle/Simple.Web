using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    public interface IHandleLogin
    {
        string ReturnUrl { set; }
        Guid UserToken { get; }
    }
}
