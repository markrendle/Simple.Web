namespace Simple.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Simple.Web.Behaviors;
    using Simple.Web.CodeGeneration;
    using Simple.Web.Hosting;
    using Simple.Web.Http;
    using Simple.Web.Mocks;

    using Xunit;

    public class CookieTests
    {
        [Fact]
        public void LoadsSingleGuidCookieUsingPropertyName()
        {
            var expected = new Guid("{6B6CB3A7-CFD7-4479-90C8-440D9F1B9F33}");
            var request = new MockRequest { Headers = new Dictionary<string, string[]> { { "Cookie", new[] { "Test=" + expected } } } };
            var context = new MockContext { Request = request };

            Run<SingleGuidCookieHandler>(context);
            Assert.Equal(expected, SingleGuidCookieHandler.TestValue);
        }

        [Fact]
        public void LoadsSingleNullGuidCookieUsingPropertyName()
        {
            var request = new MockRequest();
            var context = new MockContext { Request = request };

            Run<SingleNullableGuidCookieHandler>(context);
            Assert.False(SingleNullableGuidCookieHandler.TestValue.HasValue);
        }

        [Fact]
        public void LoadsSingleNullableGuidCookieUsingPropertyName()
        {
            var expected = new Guid("{6B6CB3A7-CFD7-4479-90C8-440D9F1B9F33}");
            var request = new MockRequest { Headers = new Dictionary<string, string[]> { { "Cookie", new[] { "Test=" + expected } } } };
            var context = new MockContext { Request = request };

            Run<SingleNullableGuidCookieHandler>(context);
            Assert.Equal(expected, SingleNullableGuidCookieHandler.TestValue);
        }

        [Fact]
        public void LoadsSingleStringCookieUsingPropertyName()
        {
            var request = new MockRequest { Headers = new Dictionary<string, string[]> { { "Cookie", new[] { "Test=Pass" } } } };
            var context = new MockContext { Request = request };

            Run<SingleStringCookieHandler>(context);
            Assert.Equal("Pass", SingleStringCookieHandler.TestValue);
        }

        //[Fact]
        //public void LoadsComplexCookieUsingPropertyName()
        //{
        //    var cookie = new MockCookie {Name = "Test", Values = new Dictionary<string, string>()};
        //    cookie.Values["Name"] = "Pass";
        //    cookie.Values["Age"] = "42";
        //    var request = new MockRequest { Cookies = new Dictionary<string, ICookie> { { "Test", cookie}} };
        //    var context = new MockContext { Request = request };

        //    var runner = new HandlerRunnerBuilder(typeof(ComplexCookieHandler), "GET").BuildRunner();
        //    var target = new ComplexCookieHandler();
        //    try
        //    {
        //        runner(target, context);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        // Content-type handling is going to throw an exception here.
        //    }
        //    Assert.Equal("Pass", target.Test.Name);
        //    Assert.Equal(42, target.Test.Age);
        //}

        [Fact]
        public void ParsesCookieValue()
        {
            const string cookie = "foo=bar";
            var value = RequestExtensions.GetCookieValue(cookie);
            Assert.Equal("bar", value);
        }

        [Fact]
        public void SetsCookieFromSingleValueProperty()
        {
            var request = new MockRequest();
            var response = new MockResponse();
            var context = new MockContext { Request = request, Response = response };
            Run<SingleStringCookieSetHandler>(context);
            string[] cookies;
            Assert.True(response.Headers.TryGetValue(HeaderKeys.SetCookie, out cookies));
            Assert.True(cookies.Any(c => c.StartsWith("Test=Pass;")));
        }

        private static void Run<T>(IContext context)
        {
            var runner = new PipelineFunctionFactory(typeof(T)).BuildAsyncRunMethod("GET");
            var info = new HandlerInfo(typeof(T), "GET");
            try
            {
                runner(context, info).Wait();
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
        }
    }

    internal class SingleStringCookieHandler : IGet
    {
        public static string TestValue;

        [Cookie]
        public string Test { get; set; }

        public Status Get()
        {
            TestValue = Test;
            return 200;
        }
    }

    internal class SingleStringCookieSetHandler : IGet
    {
        [Cookie]
        public string Test
        {
            get { return "Pass"; }
        }

        public Status Get()
        {
            return 200;
        }
    }

    internal class SingleGuidCookieHandler : IGet
    {
        public static Guid TestValue;

        [Cookie]
        public Guid Test { get; set; }

        public Status Get()
        {
            TestValue = Test;
            return 200;
        }
    }

    internal class SingleNullableGuidCookieHandler : IGet
    {
        public static Guid? TestValue;

        [Cookie]
        public Guid? Test { get; set; }

        public Status Get()
        {
            TestValue = Test;
            return 200;
        }
    }

    internal class ComplexCookieHandler : IGet
    {
        [Cookie]
        public ComplexCookie Test { get; set; }

        public Status Get()
        {
            return 200;
        }
    }

    internal class ComplexCookie
    {
        public int Age { get; set; }

        public string Name { get; set; }
    }

    public class FileMappingDictionaryTest
    {
        [Fact]
        public void AddingPathWithTrailingSlashAddsItWithout()
        {
            var target = new FileMappingDictionary();
            target.Add("/foo/", "foo.html");
            Assert.Equal("foo.html", target["/foo"].Path);
        }

        [Fact]
        public void AddingPathWithoutTrailingSlashAddsItWith()
        {
            var target = new FileMappingDictionary();
            target.Add("/foo", "foo.html");
            Assert.Equal("foo.html", target["/foo/"].Path);
        }
    }
}