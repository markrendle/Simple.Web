using System;
using System.Collections.Generic;
using Simple.Web.MediaTypeHandling;
using Simple.Web.TestHelpers.Sample;

namespace Performance.MediaTypeHandlers
{
    public static class TestData
    {
        public static readonly List<Customer> Customers = new List<Customer>
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
                                new Order(1, 101),
                                new Order(1, 106),
                                new Order(1, 107),
                                new Order(1, 108),
                                new Order(1, 109),
                                new Order(1, 110),
                                new Order(1, 111),
                                new Order(1, 112),
                                new Order(1, 113),
                                new Order(1, 114),
                                new Order(1, 115),
                                new Order(1, 116),
                                new Order(1, 117),
                                new Order(1, 118),
                                new Order(1, 119),
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
                        Orders = new List<Order>
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
                        Orders = new List<Order>
                            {
                                new Order(5, 500),
                                new Order(5, 501),
                                new Order(5, 502),
                                new Order(5, 503),
                                new Order(5, 504),
                            }
                    },
            };

        public static readonly IContent SingleContent = new Content(new Uri("http://test.com/customer/1"),
                                                                    new CustomerHandler(),
                                                                    Customers[0]);

        public static readonly IContent MultipleContent = new Content(new Uri("http://test.com/customers"),
                                                                      new CustomersHandler(),
                                                                      Customers);
    }
}