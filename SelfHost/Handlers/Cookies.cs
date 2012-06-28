using System;
using System.Collections.Generic;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.Http;

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

	public class SimpleCookie : ICookie {
		public SimpleCookie(string name, string value) {
			Name = name;
			Value = value;
		}

		public string Name { get; set; }
		public bool Secure { get; set; } 
		public bool HttpOnly{get;set;} 
		public DateTime Expires{get;set;} 
		public string Value{get;set;} 
		public IDictionary<string, string> Values{get;set;} 
		public string this[string key] {
			get { return Values[key]; }
			set { Values[key] = value; }
		}
	}

	[UriTemplate("/addcookie")]
	public class AddCookie : IPost, IMayRedirect, ISetCookies {
		public Status Post() {
			var name = "Cookie"+DateTime.Now.ToString("HH:mm:ss");
			ResponseCookies.Add(name, new SimpleCookie(name, CookieValue));
			Location = "/cookies";
			return Status.SeeOther;
		}

		public string CookieValue { get; set; }

		public IDictionary<string, ICookie> ResponseCookies { get; set; }

		public string Location { get; set; } 
	}
}