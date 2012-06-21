using System;
using System.Collections.Generic;

namespace Simple.Web.Owin
{
	public class HeaderDictionary : Dictionary<string, IEnumerable<string>>
	{
		public HeaderDictionary():base(StringComparer.OrdinalIgnoreCase){}
	}
}