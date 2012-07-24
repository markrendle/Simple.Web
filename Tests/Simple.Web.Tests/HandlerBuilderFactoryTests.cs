namespace Simple.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using CodeGeneration;
    using Xunit;

    public class HandlerBuilderFactoryTests
    {
        [Fact]
        public void CreatesTypeWithParameterlessConstructorUsingDefaultContainer()
        {
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof (ParameterlessConstructorType));
            var actual = actualFunc(new Dictionary<string, string[]>());
            Assert.IsType<ParameterlessConstructorType>(actual.Handler);
        }

        [Fact]
        public void CreatingTypeWithNoParameterlessConstructorUsingDefaultContainerThrowsInvalidOperationException()
        {
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof (NoParameterlessConstructorType));
            Assert.Throws<InvalidOperationException>(() => actualFunc(new Dictionary<string, string[]>()));
        }
    }

    class NoParameterlessConstructorType
    {
        public NoParameterlessConstructorType(int n)
        {
            
        }
    }
    class ParameterlessConstructorType
    {
        
    }
}