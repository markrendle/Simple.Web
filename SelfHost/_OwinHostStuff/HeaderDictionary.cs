using System;
using System.Collections.Generic;

namespace SelfHost
{
	public class HeaderDictionary : Dictionary<string, IEnumerable<string>>
	{
		public HeaderDictionary():base(StringComparer.OrdinalIgnoreCase){}
	}
}