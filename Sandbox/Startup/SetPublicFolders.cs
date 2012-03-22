using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox.Startup
{
    using Simple.Web;

    public class SetPublicFolders : IStartupTask
    {
        public void Run()
        {
            SimpleWeb.Configuration.PublicFolders.Add("/Scripts");
        }
    }
}