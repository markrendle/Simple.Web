namespace Simple.Web.Razor.Tests
{
    using System;
    using System.Dynamic;

    public class MockEndpoint : IEndpoint
    {
        public Status Run()
        {
            throw new NotImplementedException();
        }

        public object Output { get; set; }

        public string Title { get { return "Foo"; } }
    }
}