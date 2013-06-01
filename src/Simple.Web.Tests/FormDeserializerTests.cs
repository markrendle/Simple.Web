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
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes("Text=Hello+World!")))
            {
                actual = target.Read(s, typeof (FD)) as FD;
            }
            Assert.Equal("Hello World!", actual.Text);
        }

        [Fact]
        public void DecodesBase64String()
        {
            var target = new FormDeserializer();
            FD actual;
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes("Text=NXqAP07hSjgGiTlyCCcMoAYt4%2BNNd3qGT45HFgqOK2bqL4my1QFuGjVVa4NEQ8hXjLJEA0BERl8tNpPwEBZRng%3D%3D")))
            {
                actual = target.Read(s, typeof (FD)) as FD;
            }
            Assert.Equal("NXqAP07hSjgGiTlyCCcMoAYt4+NNd3qGT45HFgqOK2bqL4my1QFuGjVVa4NEQ8hXjLJEA0BERl8tNpPwEBZRng==", actual.Text);
        }
    }

    public class FD
    {
        public string Text { get; set; }
    }
}