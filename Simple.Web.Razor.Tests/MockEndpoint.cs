namespace Simple.Web.Razor.Tests
{
    using System;
    using System.Dynamic;

    public class MockEndpoint : IOutputEndpoint<TestModel>
    {
        public Status Run()
        {
            return 200;
        }

        public TestModel Output { get; set; }

        object IOutputEndpoint.Output { get { return Output; } }

        public Type OutputType
        {
            get { return typeof (TestModel); }
        }

        public string Title { get { return "Foo"; } }
    }
}