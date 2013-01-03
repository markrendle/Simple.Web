namespace Simple.Web.HalJson.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Links;
    using MediaTypeHandling;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class WriteTests
    {
        [Fact]
        public void WritesObjectWithLinks()
        {
            IDictionary<string, object> actual;

            var person = new Person {Name = "Marvin", Location = "Car Park"};
            var content = new Content(new PersonHandler(), person);
            var target = new HalJsonMediaTypeHandler();
            using (var stream = new MemoryStream())
            {
                target.Write(content, stream).Wait();
                stream.Position = 0;
                var text = new StreamReader(stream).ReadToEnd();
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    actual = serializer.Deserialize<Dictionary<string, object>>(jsonReader);
                }
            }

            Assert.Equal("Marvin", actual["name"]);
            Assert.Equal("Car Park", actual["location"]);
            var links = (JObject)actual["_links"];
            Assert.Equal("/person/Marvin", links["self"]["href"]);
        }
    }

    [Canonical(typeof(Person))]
    [UriTemplate("/person/{Name}")]
    public class PersonHandler
    {
        
    }
}