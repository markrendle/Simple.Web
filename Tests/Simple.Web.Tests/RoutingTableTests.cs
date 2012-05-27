namespace Simple.Web.Tests
{
    using System.Collections.Generic;
    using Routing;
    using Xunit;

    public class RoutingTableTests
    {
        [Fact]
        public void MatchesStaticUrl()
        {
            var target = new RoutingTable();
            var expected = typeof (RoutingTableTests);
            target.Add("/", expected);
            IDictionary<string, string> _;
            var actual = target.Get("/", out _);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MatchesStaticUrlNotEndingInSlash()
        {
            var target = new RoutingTable();
            var expected = typeof (RoutingTableTests);
            target.Add("/test", expected);
            IDictionary<string, string> _;
            var actual = target.Get("/test", out _);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MatchesStaticUrlEndingInSlash()
        {
            var target = new RoutingTable();
            var expected = typeof (RoutingTableTests);
            target.Add("/test", expected);
            IDictionary<string, string> _;
            var actual = target.Get("/test/", out _);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MatchesDynamicUrlWithOneVariable()
        {
            var target = new RoutingTable();
            var expected = typeof (RoutingTableTests);
            target.Add("/tests/{Id}", expected);
            IDictionary<string, string> variables;
            var actual = target.Get("/tests/1", out variables);
            Assert.Equal(expected, actual);
            Assert.Equal("1", variables["Id"]);
        }

        [Fact]
        public void MatchesDynamicUrlWithTwoVariables()
        {
            var target = new RoutingTable();
            var expected = typeof (RoutingTableTests);
            target.Add("/tests/{Year}/{Month}", expected);
            IDictionary<string, string> variables;
            var actual = target.Get("/tests/2012/2", out variables);
            Assert.Equal(expected, actual);
            Assert.Equal("2012", variables["Year"]);
            Assert.Equal("2", variables["Month"]);
        }
        
        [Fact]
        public void MatchesDynamicUrlWithTrailingValues()
        {
            var target = new RoutingTable();
            var expected = typeof (RoutingTableTests);
            target.Add("/tests/{Id}/bar", expected);
            IDictionary<string, string> variables;
            var actual = target.Get("/tests/1/bar", out variables);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MatchesDynamicUrlWithTrailingValuesAheadOfMultiValue()
        {
            var target = new RoutingTable();
            var expected = typeof(RoutingTableTests);
            target.Add("/tests/{Year}/{Month}", typeof(int));
            target.Add("/tests/{Id}/bar", expected);
            IDictionary<string, string> variables;
            var actual = target.Get("/tests/1/bar", out variables);
            Assert.Equal(expected, actual);
        }
         
        [Fact]
        public void MatchesUrlWhenTwoRegexesHaveSameNumberOfGroups()
        {
            var target = new RoutingTable();
            var expectedFoo = typeof(int);
            var expectedBar = typeof(string);
            target.Add("/tests/{Id}/foo", expectedFoo);
            target.Add("/tests/{Id}/bar", expectedBar);
            IDictionary<string, string> variables;
            Assert.Equal(expectedFoo, target.Get("/tests/1/foo", out variables));
            Assert.Equal(expectedBar, target.Get("/tests/1/bar", out variables));
        }
    }
}