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
            var actualFunc = target.BuildHandlerBuilder(typeof(ParameterlessConstructorType));
            var actual = actualFunc(new Dictionary<string, string>());
            Assert.IsType<ParameterlessConstructorType>(actual.Handler);
        }

        [Fact]
        public void CreatingTypeWithNoParameterlessConstructorUsingDefaultContainerThrowsInvalidOperationException()
        {
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof(NoParameterlessConstructorType));
            Assert.Throws<InvalidOperationException>(() => actualFunc(new Dictionary<string, string>()));
        }

        [Fact]
        public void SetsGuidPropertyCorrectly()
        {
            var guid = new Guid("FA37E0B4-2DB9-4471-BC6C-229748F417CA");
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof(GuidHolder));
            var actual = (GuidHolder)actualFunc(new Dictionary<string, string> { { "Guid", guid.ToString() } }).Handler;
            Assert.Equal(guid, actual.Guid);
        }

        [Fact]
        public void SetsEnumerableGuidPropertyCorrectly()
        {
            var guidCollection = new[]
                                     {
                                         new Guid("FA37E0B4-2DB9-4471-BC6C-229748F417CA"),
                                         new Guid("47A210D9-7E5D-480A-9300-B2CF1443C496")
                                     };
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof(EnumerableGuidHolder));
            var actual = (EnumerableGuidHolder)actualFunc(new Dictionary<string, string> { { "Guids", "FA37E0B4-2DB9-4471-BC6C-229748F417CA\t47A210D9-7E5D-480A-9300-B2CF1443C496" } }).Handler;
            Assert.Equal(guidCollection, actual.Guids);
        }

        [Fact]
        public void SetsEnumerableStringPropertyCorrectly()
        {
            var stringCollection = new[]
                                     {
                                         "hello",
                                         "world"
                                     };
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof(EnumerableStringHolder));
            var actual = (EnumerableStringHolder)actualFunc(new Dictionary<string, string> { { "Strings", "hello\tworld" } }).Handler;
            Assert.Equal((IEnumerable<string>)stringCollection, (IEnumerable<string>)actual.Strings);
        }

        [Fact]
        public void SetsEnumerableEnumPropertyCorrectly()
        {
            var enumCollection = new[]
                                     {
                                         Enterprise.Kirk,
                                         Enterprise.Spock
                                     };
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof(EnumerableEnumHolder));
            var actual = (EnumerableEnumHolder)actualFunc(new Dictionary<string, string> { { "Trekkers", "Kirk\tSpock" } }).Handler;
            Assert.Equal((IEnumerable<Enterprise>)enumCollection, (IEnumerable<Enterprise>)actual.Trekkers);
        }

        [Fact]
        public void SetsSingleEnumPropertyCorrectly()
        {
            var target = new HandlerBuilderFactory(new Configuration());
            var actualFunc = target.BuildHandlerBuilder(typeof(SingleEnumHolder));
            var actual = (SingleEnumHolder)actualFunc(new Dictionary<string, string> { { "Trekker", "Uhura" } }).Handler;
            Assert.Equal(Enterprise.Uhura, actual.Trekker);
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

    class EnumerableGuidHolder
    {
        public IEnumerable<Guid> Guids { get; set; }
    }

    class EnumerableStringHolder
    {
        public IEnumerable<string> Strings { get; set; }
    }

    enum Enterprise
    {
        Kirk,
        McCoy,
        Scott,
        Spock,
        Uhura,
        Chekov,
        Zulu
    }

    class EnumerableEnumHolder
    {
        public IEnumerable<Enterprise> Trekkers { get; set; }
    }

    class SingleEnumHolder
    {
        public Enterprise Trekker { get; set; }
    }
}