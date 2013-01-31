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
            var actual = actualFunc(new Dictionary<string, string>());
            Assert.IsType<ParameterlessConstructorType>(actual.Handler);
        }

        [Fact]
        public void CreatingTypeWithNoParameterlessConstructorUsingDefaultContainerThrowsInvalidOperationException()
        {
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof (NoParameterlessConstructorType));
            Assert.Throws<InvalidOperationException>(() => actualFunc(new Dictionary<string, string>()));
        }

        [Fact]
        public void SetsGuidPropertyCorrectly()
        {
            var guid = new Guid("FA37E0B4-2DB9-4471-BC6C-229748F417CA");
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof (GuidHolder));
            var actual = (GuidHolder)actualFunc(new Dictionary<string, string>{{"Guid",guid.ToString()}}).Handler;
            Assert.Equal(guid, actual.Guid);
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
    class GuidHolder
    {
        public Guid? Guid { get; set; }
    }
}