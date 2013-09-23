using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Web.TestHelpers;

namespace Simple.Web.Razor.Tests
{
    internal static class RazorTypeBuilderHelpers
    {
        internal static Type CreateTypeFromText(string text)
        {
            using (var reader = text.ToStreamReader())
            {
                return new RazorTypeBuilder().CreateType(reader);
            }
        }
    }
}
