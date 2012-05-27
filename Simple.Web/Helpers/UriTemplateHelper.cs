using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Helpers
{
    using System.Text.RegularExpressions;

    internal static class UriTemplateHelper
    {
        public static IEnumerable<string> ExtractVariableNames(string template)
        {
            return new Regex("{([^}]*)}").Matches(template).Cast<Match>().Select(m => m.Value.Trim('{', '}'));
        }
    }
}
