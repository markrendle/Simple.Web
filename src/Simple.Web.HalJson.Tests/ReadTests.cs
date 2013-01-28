using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.HalJson.Tests
{
    using System.IO;
    using JsonNet;
    using Xunit;

    public class ReadTests
    {
        [Fact]
        public void ReadsBasicObject()
        {
            const string source = @"{""name"":""Arthur Dent"",""location"":""Guildford""}";
            Person actual;
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(source)))
            {
                stream.Position = 0;
                var target = new HalJsonMediaTypeHandler();
                actual = (Person) target.Read(stream, typeof (Person));
            }

            Assert.Equal("Arthur Dent", actual.Name);
            Assert.Equal("Guildford", actual.Location);
        }

        [Fact]
        public void ReadsObjectWithLinks()
        {
            const string source =
                @"{""_links"": {""self"":""/person/42""}, ""name"":""Arthur Dent"",""location"":""Guildford""}";
            Person actual;
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(source)))
            {
                stream.Position = 0;
                var target = new HalJsonMediaTypeHandler();
                actual = (Person) target.Read(stream, typeof (Person));
            }

            Assert.Equal("Arthur Dent", actual.Name);
            Assert.Equal("Guildford", actual.Location);
        }
    }
}
