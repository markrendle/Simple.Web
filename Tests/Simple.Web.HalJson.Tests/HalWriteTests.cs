namespace Simple.Web.HalJson.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using JsonNet;
    using Links;
    using MediaTypeHandling;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class HalWriteTests
    {
        [Fact]
        public void WritesObjectWithLinks()
        {
            JObject actual;

            var person = new Person {Name = "Marvin", Location = "Car Park"};
            var content = new Content(new PersonHandler(), person);
            var target = new HalJsonMediaTypeHandler();
            using (var stream = new MemoryStream())
            {
                target.Write(content, stream).Wait();
                stream.Position = 0;
                var text = new StreamReader(stream).ReadToEnd();
                actual = JObject.Parse(text);
            }

            Assert.Equal("Marvin", actual["name"]);
            Assert.Equal("Car Park", actual["location"]);
            var links = (JObject)actual["_links"];
            Assert.Equal("/person/Marvin", links["self"]["href"]);
        }
        
        [Fact]
        public void WritesCollectionWithLinks()
        {
            JObject actual;

            var people = new List<Person>
                             {
                                 new Person {Name = "Marvin", Location = "Car Park"},
                                 new Person {Name = "Zaphod", Location = "The Restaurant at the End of the Universe"}
                             };
            var content = new Content(new PeopleHandler(), people);
            var target = new HalJsonMediaTypeHandler();
            using (var stream = new MemoryStream())
            {
                target.Write(content, stream).Wait();
                stream.Position = 0;
                var text = new StreamReader(stream).ReadToEnd();
                actual = JObject.Parse(text);
            }

            var array = (JArray) actual["collection"];
            Assert.Equal(2, array.Count);
            var marvin = array.First;
            Assert.Equal("Marvin", marvin["name"]);
            Assert.Equal("Car Park", marvin["location"]);
            var marvinLinks = (JObject) marvin["_links"];
            Assert.Equal("/person/Marvin", marvinLinks["self"]["href"]);
            var links = (JObject)actual["_links"];
            Assert.Equal("/people", links["self"]["href"]);
        }
    }

    [Canonical(typeof(Person))]
    [UriTemplate("/person/{Name}")]
    public class PersonHandler
    {
        
    }

    [Canonical(typeof(IEnumerable<Person>))]
    [UriTemplate("/people")]
    public class PeopleHandler
    {
        
    }
}