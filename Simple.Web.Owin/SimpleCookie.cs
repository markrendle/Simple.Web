using System;
using System.Collections.Generic;
using Simple.Web.Http;

namespace Simple.Web.Owin {
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
}