using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    using Http;

    public interface IExceptionHandler
    {
        Status Handle(Exception exception, IContext context);
    }
}
