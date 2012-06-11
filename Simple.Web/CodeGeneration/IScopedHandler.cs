using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.CodeGeneration
{
    public interface IScopedHandler: IDisposable
    {
        object Handler { get; }
    }
}
