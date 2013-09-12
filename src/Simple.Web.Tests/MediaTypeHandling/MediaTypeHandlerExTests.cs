namespace Simple.Web.Tests.MediaTypeHandling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Simple.Web.MediaTypeHandling;
    using Xunit;

    public class MediaTypeHandlerExTests
    {
        [Fact]
        public void GetContentTypeForHandlersWithMatchingWildcardReturnCorrectContentType()
        {
            var handler = new GenericJSONHandler();
            var acceptedTypes = new List<string> {"application/something+json", "text/html"};
            var contentType = handler.GetContentType(acceptedTypes);
            Assert.Equal("application/something+json", contentType);
        }

        [Fact]
        public void GetContentTypeForHandlersWithNonMatchingWildcardReturnCorrectContentType()
        {
            var handler = new GenericJSONWithFallbackHandler();
            var acceptedTypes = new List<string> {"application/something+xml", "text/html"};
            var contentType = handler.GetContentType(acceptedTypes);
            Assert.Equal("text/html", contentType);
        }

        [Fact]
        public void GetContentTypeForHandlersMatchExplicitlyReturnCorrectContentType()
        {
            var handler = new PlainHTMLHandler();
            var acceptedTypes = new List<string> {"application/something+json", "text/html"};
            var contentType = handler.GetContentType(acceptedTypes);
            Assert.Equal("text/html", contentType);
        }

        [MediaTypes("application/*+json")]
        private class GenericJSONHandler : IMediaTypeHandler
        {
            public Task<T> Read<T>(Stream inputStream)
            {
                throw new NotImplementedException();
            }

            public Task Write<T>(IContent content, Stream outputStream)
            {
                throw new NotImplementedException();
            }
        }

        [MediaTypes("application/*+json", "text/html")]
        private class GenericJSONWithFallbackHandler : IMediaTypeHandler
        {
            public Task<T> Read<T>(Stream inputStream)
            {
                throw new NotImplementedException();
            }

            public Task Write<T>(IContent content, Stream outputStream)
            {
                throw new NotImplementedException();
            }
        }

        [MediaTypes("text/html")]
        private class PlainHTMLHandler : IMediaTypeHandler
        {
            public Task<T> Read<T>(Stream inputStream)
            {
                throw new NotImplementedException();
            }

            public Task Write<T>(IContent content, Stream outputStream)
            {
                throw new NotImplementedException();
            }
        }
    }
}