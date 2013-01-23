namespace Simple.Web.Owin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    internal static class QueryStringParser
    {
        public static IDictionary<string,string[]> Parse(string queryString)
        {
            var workingDictionary = new Dictionary<string, List<string>>();
            var chunks = queryString.Split('&');
            foreach (var chunk in chunks)
            {
                var parts = chunk.Split('=');
                if (!workingDictionary.ContainsKey(parts[0]))
                {
                    workingDictionary.Add(parts[0], new List<string>());
                }
                if (parts.Length == 2)
                {
                    workingDictionary[parts[0]].Add(HttpUtility.UrlDecode(parts[1]));
                }
                else
                {
                    workingDictionary[parts[0]].Add(string.Empty);
                }
            }

            return workingDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }
    }
}