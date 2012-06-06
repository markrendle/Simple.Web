using System;
using System.Collections.Generic;
using Simple.Web.Behaviors;
using Simple.Web.CodeGeneration;
using Simple.Web.Http;
using Simple.Web.Mocks;
using Xunit;

namespace Simple.Web.Tests
{
    public class CookieTests
    {
        [Fact]
        public void LoadsSingleStringCookieUsingPropertyName()
        {
            var request = new MockRequest
                {Cookies = new Dictionary<string, ICookie> {{"Test", new MockCookie {Name = "Test", Value = "Pass"}}}};
            var context = new MockContext {Request = request};

            var runner = new HandlerRunnerBuilder(typeof (SingleStringCookieHandler), "GET").BuildRunner();
            var target = new SingleStringCookieHandler();
            try
            {
                runner(target, context);
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
            Assert.Equal("Pass", target.Test);
        }
        
        [Fact]
        public void LoadsSingleGuidCookieUsingPropertyName()
        {
            var expected = new Guid("{6B6CB3A7-CFD7-4479-90C8-440D9F1B9F33}");
            var request = new MockRequest
                {Cookies = new Dictionary<string, ICookie> {{"Test", new MockCookie {Name = "Test", Value = expected.ToString()}}}};
            var context = new MockContext {Request = request};

            var runner = new HandlerRunnerBuilder(typeof (SingleGuidCookieHandler), "GET").BuildRunner();
            var target = new SingleGuidCookieHandler();
            try
            {
                runner(target, context);
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
            Assert.Equal(expected, target.Test);
        }
        
        [Fact]
        public void LoadsSingleNullableGuidCookieUsingPropertyName()
        {
            var expected = new Guid("{6B6CB3A7-CFD7-4479-90C8-440D9F1B9F33}");
            var request = new MockRequest
                {Cookies = new Dictionary<string, ICookie> {{"Test", new MockCookie {Name = "Test", Value = expected.ToString()}}}};
            var context = new MockContext {Request = request};

            var runner = new HandlerRunnerBuilder(typeof (SingleNullableGuidCookieHandler), "GET").BuildRunner();
            var target = new SingleNullableGuidCookieHandler();
            try
            {
                runner(target, context);
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
            Assert.Equal(expected, target.Test);
        }
        
        [Fact]
        public void LoadsSingleNullGuidCookieUsingPropertyName()
        {
            var request = new MockRequest {Cookies = new Dictionary<string, ICookie>()};
            var context = new MockContext {Request = request};

            var runner = new HandlerRunnerBuilder(typeof (SingleNullableGuidCookieHandler), "GET").BuildRunner();
            var target = new SingleNullableGuidCookieHandler();
            try
            {
                runner(target, context);
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
            Assert.False(target.Test.HasValue);
        }

        [Fact]
        public void LoadsComplexCookieUsingPropertyName()
        {
            var cookie = new MockCookie {Name = "Test", Values = new Dictionary<string, string>()};
            cookie.Values["Name"] = "Pass";
            cookie.Values["Age"] = "42";
            var request = new MockRequest { Cookies = new Dictionary<string, ICookie> { { "Test", cookie}} };
            var context = new MockContext { Request = request };

            var runner = new HandlerRunnerBuilder(typeof(ComplexCookieHandler), "GET").BuildRunner();
            var target = new ComplexCookieHandler();
            try
            {
                runner(target, context);
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
            Assert.Equal("Pass", target.Test.Name);
            Assert.Equal(42, target.Test.Age);
        }

        [Fact]
        public void SetsCookieFromSingleValueProperty()
        {
            var request = new MockRequest();
            var response = new MockResponse();
            var context = new MockContext {Request = request, Response = response};
            var handler = new SingleStringCookieHandler {Test = "Pass"};
            Run(handler, context);
            Assert.True(response.Cookies.ContainsKey("Test"));
            Assert.Equal("Pass", response.Cookies["Test"]);
        }
        
        [Fact]
        public void SetsCookieFromComplexProperty()
        {
            var request = new MockRequest();
            var response = new MockResponse();
            var context = new MockContext {Request = request, Response = response};
            var handler = new ComplexCookieHandler() { Test = new ComplexCookie { Name = "Pass", Age = 42 } };
            Run(handler, context);
            Assert.True(response.Cookies.ContainsKey("Test"));
            var dict = response.Cookies["Test"] as IDictionary<string, string>;
            Assert.NotNull(dict);
            Assert.Equal("Pass", dict["Name"]);
        }

        private static void Run<T>(T handler, IContext context)
        {
            var runner = new HandlerRunnerBuilder(typeof(T), "GET").BuildRunner();
            try
            {
                runner(handler, context);
            }
            catch (ArgumentNullException)
            {
                // Content-type handling is going to throw an exception here.
            }
        }
    }

    class SingleStringCookieHandler : IGet
    {
        public Status Get()
        {
            return 200;
        }

        [Cookie]
        public string Test { get; set; }
    }
    
    class SingleGuidCookieHandler : IGet
    {
        public Status Get()
        {
            return 200;
        }

        [Cookie]
        public Guid Test { get; set; }
    }
    
    class SingleNullableGuidCookieHandler : IGet
    {
        public Status Get()
        {
            return 200;
        }

        [Cookie]
        public Guid? Test { get; set; }
    }

    class ComplexCookieHandler : IGet
    {
        public Status Get()
        {
            return 200;
        }

        [Cookie]
        public ComplexCookie Test { get; set; }
    }

    class ComplexCookie
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}