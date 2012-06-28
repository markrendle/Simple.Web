using System;
using System.Collections.Generic;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.Http;
using Simple.Web.Owin;

namespace SelfHost
{
	[UriTemplate("/cookies")]
    public class GetCookies : IGet, IReadCookies
    {
        public Status Get()
        {
            return 200;
        }

		public IEnumerable<string> Cookies {
			get {
				return RequestCookies.Keys;
			}
		}

		public IDictionary<string, ICookie> RequestCookies { get; set; }
    }

	[UriTemplate("/addcookie")]
	public class AddCookie : IPost, IMayRedirect, ISetCookies, IInput<CookieText> {
		public Status Post() {
			var name = "Cookie" + DateTime.Now.ToString("HH:mm:ss");

			ResponseCookies.Add(name, new SimpleCookie(name, Input.CookieValue));

			Location = "/cookies";
			return Status.SeeOther;
		}

		public IDictionary<string, ICookie> ResponseCookies { get; set; }

		public string Location { get; set; }

		public CookieText Input { get; set; } 
	}

	public class CookieText {
		public string CookieValue { get; set; }
	}
}