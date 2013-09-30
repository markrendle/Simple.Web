namespace Simple.Web.Razor.Tests
{
    using System;

    using Simple.Web.TestHelpers;

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