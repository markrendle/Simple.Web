namespace Simple.Web.JsonNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json.Linq;

    using Simple.Web.MediaTypeHandling;
    using Simple.Web.TestHelpers.Sample;

    using Xunit;

    public class HalJsonMediaTypeHandlerTests
    {
        [Fact]
        public void ReadsBasicObject()
        {
            const string source = @"{""name"":""Arthur Dent"",""location"":""Guildford""}";
            Person actual;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                stream.Position = 0;
                var target = new HalJsonMediaTypeHandler();
                actual = target.Read<Person>(stream).Result;
            }

            Assert.Equal("Arthur Dent", actual.Name);
            Assert.Equal("Guildford", actual.Location);
        }

        [Fact]
        public void ReadsObjectWithLinks()
        {
            const string source = @"{""_links"": {""self"":""/person/42""}, ""name"":""Arthur Dent"",""location"":""Guildford""}";
            Person actual;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                stream.Position = 0;
                var target = new HalJsonMediaTypeHandler();
                actual = target.Read<Person>(stream).Result;
            }

            Assert.Equal("Arthur Dent", actual.Name);
            Assert.Equal("Guildford", actual.Location);
        }

        [Fact]
        public void WritesCollectionWithLinks()
        {
            JObject actual;

            var people = new List<Person>
                {
                    new Person { Name = "Marvin", Location = "Car Park" },
                    new Person { Name = "Zaphod", Location = "The Restaurant at the End of the Universe" }
                };
            var content = new Content(new Uri("http://test.com/people"), new PeopleHandler(), people);
            var target = new HalJsonMediaTypeHandler();
            using (var stream = new MemoryStream())
            {
                target.Write<IEnumerable<Person>>(content, stream).Wait();
                stream.Position = 0;
                string text = new StreamReader(stream).ReadToEnd();
                actual = JObject.Parse(text);
            }

            var array = (JArray)actual["collection"];
            Assert.Equal(2, array.Count);
            JToken marvin = array.First;
            Assert.Equal("Marvin", marvin["name"]);
            Assert.Equal("Car Park", marvin["location"]);
            var marvinLinks = (JObject)marvin["_links"];
            Assert.Equal("/person/Marvin", marvinLinks["self"]["href"]);
            var links = (JObject)actual["_links"];
            Assert.Equal("/people", links["self"]["href"]);
        }

        [Fact]
        public void WritesObjectWithLinks()
        {
            JObject actual;

            var person = new Person { Name = "Marvin", Location = "Car Park" };
            var content = new Content(new Uri("http://test.com/person/Marvin"), new PersonHandler(), person);
            var target = new HalJsonMediaTypeHandler();
            using (var stream = new MemoryStream())
            {
                target.Write<Person>(content, stream).Wait();
                stream.Position = 0;
                string text = new StreamReader(stream).ReadToEnd();
                actual = JObject.Parse(text);
            }

            Assert.Equal("Marvin", actual["name"]);
            Assert.Equal("Car Park", actual["location"]);
            var links = (JObject)actual["_links"];
            Assert.Equal("/person/Marvin", links["self"]["href"]);
        }
    }
}