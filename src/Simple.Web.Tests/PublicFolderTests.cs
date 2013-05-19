namespace Simple.Web.Tests
{
    using Xunit;

    public class PublicFolderTests
    {
        [Fact]
        public void CreatesPublicFolderFromString()
        {
            const string expected = "/css";
            PublicFolder actual = expected;
            Assert.Equal(expected, actual.Path);
            Assert.Equal(expected, actual.Alias);
        }

        [Fact]
        public void RewritesPath()
        {
            var target = new PublicFolder("/js", "/js-1.1.0.0");
            var actual = target.RewriteAliasToPath("/js-1.1.0.0/app.js");
            Assert.Equal("/js/app.js", actual);
        }
    }
}