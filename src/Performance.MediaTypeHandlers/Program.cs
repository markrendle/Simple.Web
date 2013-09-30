namespace Performance.MediaTypeHandlers
{
    using System;
    using System.Collections.Generic;

    using Simple.Web;
    using Simple.Web.JsonNet;
    using Simple.Web.MediaTypeHandling;
    using Simple.Web.TestHelpers;
    using Simple.Web.TestHelpers.Sample;
    using Simple.Web.Xml;
    using Simple.Web.Xml.Tests;

    internal static class Program
    {
        private const int Iterations = 5000;

        private static void Main(string[] args)
        {
            SimpleWeb.Configuration.Container = new XmlTestContainer();

            Console.WriteLine("Performance testing of MediaTypeHandlers");
            Console.WriteLine("\tS: Single Customer with 20 Orders");
            Console.WriteLine("\tM: Five Customers with at least Five Orders");
            Console.WriteLine();
            Console.WriteLine("Testing over {0} iterations...", Iterations);
            Console.WriteLine();

            TestHandler("ExplicitXml    ", () => new ExplicitXmlMediaTypeHandler());
            TestHandler("DataContractXml", () => new DataContractXmlMediaTypeHandler());
            TestHandler("JsonNet        ", () => new JsonMediaTypeHandler());
            TestHandler("HalJsonNet     ", () => new HalJsonMediaTypeHandler());
            TestHandler("JsonNet W/Links", () => new JsonMediaTypeHandlerWithDeepLinks());
            TestHandler("JsonFX         ", () => new Simple.Web.JsonFx.JsonMediaTypeHandler());

            Console.WriteLine();
            Console.WriteLine("Done. Press 'Enter' to exit.");
            Console.ReadLine();
        }

        private static void TestHandler(string name, Func<IMediaTypeHandler> ctor)
        {
            Console.Write("{0}\t\tS: ", name);
            double average = CodeExecutionTimer.Average(Iterations,
                                                        () =>
                                                        {
                                                            IMediaTypeHandler handler = ctor();
                                                            TestWriteSingle(handler);
                                                        });
            Console.Write("{0:0.0000000000}s\tM: ", average);
            average = CodeExecutionTimer.Average(Iterations,
                                                 () =>
                                                 {
                                                     IMediaTypeHandler handler = ctor();
                                                     TestWriteMultiple(handler);
                                                 });
            Console.WriteLine("{0:0.0000000000}s", average);
        }

        private static void TestWriteMultiple(IMediaTypeHandler handler)
        {
            string actual;
            using (var stream = new StringBuilderStream())
            {
                handler.Write<IEnumerable<Customer>>(TestData.MultipleContent, stream).Wait();
                actual = stream.StringValue;
            }
            if (actual == null)
            {
                throw new Exception("No Output!");
            }
        }

        private static void TestWriteSingle(IMediaTypeHandler handler)
        {
            string actual;
            using (var stream = new StringBuilderStream())
            {
                handler.Write<Customer>(TestData.SingleContent, stream).Wait();
                actual = stream.StringValue;
            }
            if (actual == null)
            {
                throw new Exception("No Output!");
            }
        }
    }
}