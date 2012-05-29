using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandbox.Models
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}