using System;
using System.Linq;
using System.Collections.Generic;

namespace Simple.Web.Owin
{
	public class HeaderDictionary : Dictionary<string, IEnumerable<string>>
	{
		public HeaderDictionary():base(StringComparer.OrdinalIgnoreCase){}

		public HeaderDictionary Include(string headerName, string value)
		{
			if (ContainsKey(headerName))
			{
				this[headerName] = this[headerName].Union(new[]{value}).ToArray();
			} else
			{
				Add(headerName, new[]{value});
			}
			return this;
		}

		public HeaderDictionary Set(string headerName, string value)
		{
			if (ContainsKey(headerName))
			{
				this[headerName] = new[]{value};
			} else
			{
				Add(headerName, new[]{value});
			}
			return this;
		}
	}
}