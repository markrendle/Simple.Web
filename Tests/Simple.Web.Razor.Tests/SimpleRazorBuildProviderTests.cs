namespace Simple.Web.Razor.Tests
{
    using System.Web.Compilation;
    using Xunit;

    public class SimpleRazorBuildProviderTests
    {
        const string markup = @"@handler Sandbox.GetForm
<!DOCTYPE html>
<html>
<head><title>@Handler.Title</title></head>
<body>
<h1>@Handler.Title</h1>
</body>
</html>";

        [Fact]
        public void Works()
        {
            var target = new SimpleRazorBuildProvider();
        }
    }
}
