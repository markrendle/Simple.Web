namespace Simple.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using CodeGeneration;
    using Xunit;

    public class EndpointBuilderFactoryTests
    {
        [Fact]
        public void CreatesTypeWithParameterlessConstructorUsingDefaultContainer()
        {
            var target = new EndpointBuilderFactory(new Configuration());
            var actualFunc = target.BuildEndpointBuilder(typeof (ParameterlessConstructorType));
            var actual = actualFunc(new Dictionary<string, string>());
            Assert.IsType<ParameterlessConstructorType>(actual);
        }

        [Fact]
        public void CreatingTypeWithNoParameterlessConstructorUsingDefaultContainerThrowsInvalidOperationException()
        {
            var target = new EndpointBuilderFactory(new Configuration());
            var actualFunc = target.BuildEndpointBuilder(typeof (NoParameterlessConstructorType));
            Assert.Throws<InvalidOperationException>(() => actualFunc(new Dictionary<string, string>()));
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