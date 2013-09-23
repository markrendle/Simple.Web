using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simple.Web.TestHelpers
{
    public static class StringExtensions
    {
        public static StreamReader ToStreamReader(this string input)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return new StreamReader(stream);
        }
    }
}
