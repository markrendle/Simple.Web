﻿namespace Simple.Web.CodeGeneration.Tests
{
    using System.Collections.Generic;
    using Behaviors;
    using Http;
    using Mocks;
    using Xunit;

    public class GenericTest
    {
        [Fact]
        public void CallsFooImplementation()
        {
            var target = new HandlerRunnerBuilder(typeof(StringFoo), "GET").BuildRunner();
            var context = new Mocks.MockContext { Request = new MockRequest { Headers = new Dictionary<string, string[]> { { HeaderKeys.Accept, new[] { "text/html" } } } } };
            var foo = new StringFoo();
            target(foo, context);
            Assert.True(foo.Called);
        }
    }

    class StringFoo : IGet, IFoo<string>
    {
        public string Value { get; set; }
        public bool Called { get; set; }
        public Status Get()
        {
            return new Status();
        }
    }

    [RequestBehavior(typeof(GenericFooImpl))]
    public interface IFoo<T>
    {
        T Value { get; set; }
        bool Called { get; set; }
    }

    public static class GenericFooImpl
    {
        public static bool Called;
        public static void Impl<T>(IFoo<T> foo, IContext context)
        {
            foo.Called = true;
        }
    }
}