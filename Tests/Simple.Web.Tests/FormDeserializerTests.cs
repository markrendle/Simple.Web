using System.IO;
using System.Text;
using Simple.Web.MediaTypeHandling;
using Xunit;

namespace Simple.Web.Tests
{
    public class FormDeserializerTests
    {
        [Fact]
        public void DecodesUrlEncodedStrings()
        {
            var target = new FormDeserializer();
            FD actual;
            using (var s = new MemoryStream(Encoding.Default.GetBytes("Text=Hello+World!")))
            {
                actual = target.Read(s, typeof (FD)) as FD;
            }
            Assert.Equal("Hello World!", actual.Text);
        }
    }

    public class FD
    {
        public string Text { get; set; }
    }
}