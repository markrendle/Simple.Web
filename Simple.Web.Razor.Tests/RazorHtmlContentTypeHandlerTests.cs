namespace Simple.Web.Razor.Tests
{
    using System;
    using System.IO;
    using Xunit;

    public class RazorHtmlContentTypeHandlerTests
    {
        [Fact]
        public void Renders()
        {
            const string templateText =
                @"@model Simple.Web.Razor.Tests.TestModel
<!DOCTYPE html><html><head><title>@Var.Title</title></head><body>@Model.Text</body></html>";

            const string expected = @"<!DOCTYPE html><html><head><title>Foo</title></head><body>Test Text</body></html>";

            Type type;
            using (var reader = new StringReader(templateText))
            {
                type = new RazorTypeBuilder().CreateType(reader);
            }

            var endpoint = new MockEndpoint {Output = new TestModel {Text = "Test Text"}};

            var writer = new StringWriter();
            RazorHtmlContentTypeHandler.RenderView(endpoint, writer, type);
            Assert.Equal(expected, writer.ToString().Trim());
        }
    }
}