using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SelfHost
{
	public static class StringExtensions
	{
		public static IDictionary<string, string> ToQueryDictionary(this string rawString)
		{
			var pairs = rawString.Split(new []{'&'}, StringSplitOptions.RemoveEmptyEntries);
			var d = new Dictionary<string, string>();
			if (pairs.Length < 1) return d;

			foreach (var pair in pairs)
			{
				var x = pair.Split(new[] {'='}, 2);
				var k = x[0];
				var v = x.Length > 1 ? x[1] : "";
				d.Add(k, v);
			}
			return d;
		}

		public static NameValueCollection ToNameValueCollection(this IDictionary<string, IEnumerable<string>> headerDict)
		{
			var nvc = new NameValueCollection(headerDict.Count);
			foreach (var key in headerDict.Keys.Where(key => key != "Cookie"))
			{
				nvc.Add(key, headerDict[key].Aggregate("", (a,b) => a+","+b));
			}
			return nvc;
		}
	}
}
