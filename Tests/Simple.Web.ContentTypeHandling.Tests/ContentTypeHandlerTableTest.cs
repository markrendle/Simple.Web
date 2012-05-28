using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.ContentTypeHandling.Tests
{
    using System.IO;
    using Xunit;

    public class ContentTypeHandlerTableTest
    {
        [Fact]
        public void FindsHandlerForDirectType()
        {
            var table = new ContentTypeHandlerTable();
            var actual = table.GetContentTypeHandler(ContentType.Json);
            Assert.NotNull(actual);
            Assert.IsType<TestContentTypeHandler>(actual);
        }

        [Fact]
        public void FindsHandlerForCustomType()
        {
            var table = new ContentTypeHandlerTable();
            var actual = table.GetContentTypeHandler("application/vnd.test.towel+json");
            Assert.NotNull(actual);
            Assert.IsType<TestContentTypeHandler>(actual);
        }
        
        [Fact]
        public void FindsHandlerForDirectTypeList()
        {
            var table = new ContentTypeHandlerTable();
            string matchedType;
            var actual = table.GetContentTypeHandler(new[] {"application/foo", ContentType.Json}, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<TestContentTypeHandler>(actual);
            Assert.Equal(ContentType.Json, matchedType);
        }

        [Fact]
        public void FindsHandlerForCustomTypeList()
        {
            var table = new ContentTypeHandlerTable();
            string matchedType;
            const string customType = "application/vnd.test.towel+json";
            var actual = table.GetContentTypeHandler(new[] {"application/foo", customType}, out matchedType);
            Assert.NotNull(actual);
            Assert.IsType<TestContentTypeHandler>(actual);
            Assert.Equal(customType, matchedType);
        }
    }

    [ContentTypes(ContentType.Json, "application/*+json")]
    public class TestContentTypeHandler : IContentTypeHandler
    {
        public object Read(Stream inputStream, Type inputType)
        {
            throw new NotImplementedException();
        }

        public void Write(IContent content, Stream outputStream)
        {
            throw new NotImplementedException();
        }
    }
}
