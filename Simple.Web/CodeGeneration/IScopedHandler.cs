using System;

namespace Simple.Web.CodeGeneration
{
    public interface IScopedHandler: IDisposable
    {
        object Handler { get; }
    }
}
