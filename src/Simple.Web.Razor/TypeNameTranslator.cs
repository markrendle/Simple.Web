namespace Simple.Web.Razor
{
    using System.Collections.Generic;
    using System.Text;

    internal static class TypeNameTranslator
    {
        public static string CSharpNameToTypeName(string csharpName)
        {
            var strings = new Stack<StringBuilder>();
            var counts = new Stack<int>();
            var builder = new StringBuilder();
            int genericTypeCount = 0;
            foreach (var c in csharpName)
            {
                if (c == '<')
                {
                    counts.Push(genericTypeCount);
                    genericTypeCount = 1;
                    strings.Push(builder);
                    builder = new StringBuilder();
                }
                else if (c == '>')
                {
                    var target = strings.Pop();
                    target.AppendFormat("`{0}[{1}]", genericTypeCount, builder);
                    genericTypeCount = counts.Pop();
                    builder = target;
                }
                else
                {
                    if (c == ',')
                    {
                        ++genericTypeCount;
                    }
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
}