namespace Simple.Web.Tests.MediaTypeHandling
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Simple.Web.MediaTypeHandling;

    using Xunit;

    public class ContentTypeHandlerTableTest
    {
        [Fact]
        public void FindsHandlerForCustomType()
        {
            var table = new MediaTypeHandlerTable();
            var actual = table.GetMediaTypeHandler("application/vnd.test.towel+json");
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
        }

        [Fact]
        public void FindsHandlerForCustomTypeList()
        {
            var table = new MediaTypeHandlerTable();
            string matchedType;
            const string customType = "application/vnd.test.towel+json";
            var actual = table.GetMediaTypeHandler(new[] { "application/foo", customType }, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
            Assert.Equal(customType, matchedType);
        }

        [Fact]
        public void FindsHandlerForCustomTypeWithCharset()
        {
            var table = new MediaTypeHandlerTable();
            var actual = table.GetMediaTypeHandler("application/vnd.test.towel+json; charset=utf-8");
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
        }

        [Fact]
        public void FindsHandlerForDirectType()
        {
            var table = new MediaTypeHandlerTable();
            var actual = table.GetMediaTypeHandler(MediaType.Json);
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
        }

        [Fact]
        public void FindsHandlerForDirectTypeList()
        {
            var table = new MediaTypeHandlerTable();
            string matchedType;
            var actual = table.GetMediaTypeHandler(new[] { "application/foo", MediaType.Json }, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
            Assert.Equal(MediaType.Json, matchedType);
        }

        [Fact]
        public void FindsHandlerForHal()
        {
            var table = new MediaTypeHandlerTable();
            string matchedType;
            const string customType = "application/hal+json";
            var actual = table.GetMediaTypeHandler(new[] { customType }, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<HalMediaTypeHandler>(actual);
            Assert.Equal(customType, matchedType);
        }
    }

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class TestMediaTypeHandler : IMediaTypeHandler
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

    [MediaTypes("application/hal+json")]
    public class HalMediaTypeHandler : IMediaTypeHandler
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