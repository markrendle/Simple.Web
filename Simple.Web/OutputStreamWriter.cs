namespace Simple.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class OutputStreamWriter
    {
        public static IOutputWriter GetWriter(string[] acceptTypes, IEndpoint endpoint)
        {
            if (acceptTypes == null || acceptTypes.Length == 0)
            {
                return new FallbackWriter();
            }

            return new FallbackWriter();
        }
    }

    internal class FallbackWriter : IOutputWriter
    {
        public string ContentType
        {
            get { return "text/text"; }
        }

        public void Write(Stream outputStream, object output)
        {
            using (var writer = new StreamWriter(outputStream))
            {
                writer.Write(output.ToString());
            }
        }
    }
}