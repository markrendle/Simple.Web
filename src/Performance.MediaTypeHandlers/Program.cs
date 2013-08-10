using System;
using System.Collections.Generic;
using Simple.Web;
using Simple.Web.MediaTypeHandling;
using Simple.Web.TestHelpers;
using Simple.Web.TestHelpers.Sample;
using Simple.Web.Xml;
using Simple.Web.Xml.Tests;

namespace Performance.MediaTypeHandlers
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const int iterations = 5000;
            SimpleWeb.Configuration.Container = new XmlTestContainer();

            Console.WriteLine("Creating Content");
            IContent content = CreateContent();
            Console.WriteLine();

            Console.WriteLine("Testing ExplicitXml.Write<T> over {0} iterations...", iterations);
            var explicitAverage = CodeExecutionTimer.Average(iterations, () => ExplicitXmlWrite(content));
            Console.WriteLine("Average: {0}s", explicitAverage);
            Console.WriteLine();

            Console.WriteLine("Testing DataContractXml.Write<T> over {0} iterations...", iterations);
            var dataContractAverage = CodeExecutionTimer.Average(iterations, () => DataContractXmlWrite(content));
            Console.WriteLine("Average: {0}s", dataContractAverage);
            Console.WriteLine();

            Console.WriteLine("Testing JsonNet.Write<T> over {0} iterations...", iterations);
            var jsonNetAverage = CodeExecutionTimer.Average(iterations, () => JsonNetWrite(content));
            Console.WriteLine("Average: {0}s", jsonNetAverage);
            Console.WriteLine();

            Console.WriteLine("Testing HalJsonNet.Write<T> over {0} iterations...", iterations);
            var halJsonNetAverage = CodeExecutionTimer.Average(iterations, () => HalJsonNetWrite(content));
            Console.WriteLine("Average: {0}s", halJsonNetAverage);
            Console.WriteLine();

            Console.WriteLine("Testing JsonFX.Write<T> over {0} iterations...", iterations);
            var jsonFxAverage = CodeExecutionTimer.Average(iterations, () => JsonFxWrite(content));
            Console.WriteLine("Average: {0}s", jsonFxAverage);
            Console.WriteLine();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static IContent CreateContent()
        {
            var customers = new List<Customer>
                {
                    new Customer(1)
                        {
                            Orders = new List<Order>
                                {
                                    new Order(1, 100),
                                    new Order(1, 101),
                                    new Order(1, 102),
                                    new Order(1, 103),
                                    new Order(1, 104),
                                }
                        },
                    new Customer(2)
                        {
                            Orders = new List<Order>
                                {
                                    new Order(2, 200),
                                    new Order(2, 201),
                                    new Order(2, 202),
                                    new Order(2, 203),
                                    new Order(2, 204),
                                }
                        },
                    new Customer(3)
                        {
                            Orders = new List<Order>
                                {
                                    new Order(3, 300),
                                    new Order(3, 301),
                                    new Order(3, 302),
                                    new Order(3, 303),
                                    new Order(3, 304),
                                }
                        },
                    new Customer(4)
                        {
                            Orders = new List<Order>()
                                {
                                    new Order(4, 400),
                                    new Order(4, 401),
                                    new Order(4, 402),
                                    new Order(4, 403),
                                    new Order(4, 404),
                                }
                        },
                    new Customer(5)
                        {
                            Orders = new List<Order>()
                                {
                                    new Order(5, 500),
                                    new Order(5, 501),
                                    new Order(5, 502),
                                    new Order(5, 503),
                                    new Order(5, 504),
                                    new Order(5, 505),
                                    new Order(5, 506),
                                    new Order(5, 507),
                                    new Order(5, 508),
                                    new Order(5, 509),
                                    new Order(5, 510),
                                    new Order(5, 511),
                                    new Order(5, 512),
                                    new Order(5, 513),
                                    new Order(5, 514),
                                    new Order(5, 515),
                                    new Order(5, 516),
                                    new Order(5, 517),
                                    new Order(5, 518),
                                    new Order(5, 519),
                                }
                        },
                };
            return new Content(new Uri("http://test.com/customers"), new CustomersHandler(), customers);
        }

        private static void ExplicitXmlWrite(IContent content)
        {
            var handler = new ExplicitXmlMediaTypeHandler();
            TestWrite(handler, content);
        }

        private static void DataContractXmlWrite(IContent content)
        {
            var handler = new DataContractXmlMediaTypeHandler();
            TestWrite(handler, content);
        }

        private static void JsonNetWrite(IContent content)
        {
            var handler = new Simple.Web.JsonNet.JsonMediaTypeHandler();
            TestWrite(handler, content);
        }

        private static void HalJsonNetWrite(IContent content)
        {
            var handler = new Simple.Web.JsonNet.HalJsonMediaTypeHandler();
            TestWrite(handler, content);
        }

        private static void JsonFxWrite(IContent content)
        {
            var handler = new Simple.Web.JsonFx.JsonMediaTypeHandler();
            TestWrite(handler, content);
        }
        
        private static void TestWrite(IMediaTypeHandler handler, IContent content)
        {
            string actual;
            using (var stream = new StringBuilderStream())
            {
                handler.Write<Customer>(content, stream).Wait();
                actual = stream.StringValue;
            }
            if (actual == null)
            {
                throw new Exception("No Output!");
            }
        }
    }
}