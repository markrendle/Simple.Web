using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.ContentTypeHandling.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using MediaTypeHandling;
    using Xunit;

    public class ContentTypeHandlerTableTest
    {
        [Fact]
        public void FindsHandlerForDirectType()
        {
            var table = new MediaTypeHandlerTable();
            var actual = table.GetMediaTypeHandler(MediaType.Json);
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
        }

        [Fact]
        public void FindsHandlerForCustomType()
        {
            var table = new MediaTypeHandlerTable();
            var actual = table.GetMediaTypeHandler("application/vnd.test.towel+json");
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
        }
        
        [Fact]
        public void FindsHandlerForDirectTypeList()
        {
            var table = new MediaTypeHandlerTable();
            string matchedType;
            var actual = table.GetMediaTypeHandler(new[] {"application/foo", MediaType.Json}, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
            Assert.Equal(MediaType.Json, matchedType);
        }

        [Fact]
        public void FindsHandlerForCustomTypeList()
        {
            var table = new MediaTypeHandlerTable();
            string matchedType;
            const string customType = "application/vnd.test.towel+json";
            var actual = table.GetMediaTypeHandler(new[] {"application/foo", customType}, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<TestMediaTypeHandler>(actual);
            Assert.Equal(customType, matchedType);
        }
    }

    [MediaTypes(MediaType.Json, "application/*+json")]
    public class TestMediaTypeHandler : IMediaTypeHandler
    {
        public object Read(Stream inputStream, Type inputType)
        {
            throw new NotImplementedException();
        }

        public Task Write(IContent content, Stream outputStream)
        {
            throw new NotImplementedException();
        }
    }
}
