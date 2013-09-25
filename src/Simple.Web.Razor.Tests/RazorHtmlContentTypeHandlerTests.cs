namespace Simple.Web.Razor.Tests
{
    using System.IO;
    using Xunit;

    public class RazorHtmlContentTypeHandlerTests
    {
        [Fact]
        public void Renders()
        {
            const string templateText =
                @"@model Simple.Web.Razor.Tests.TestModel
<!DOCTYPE html><html><head><title>@Handler.Title</title></head><body>@Model.Text</body></html>";

            const string expected = @"<!DOCTYPE html><html><head><title>Foo</title></head><body>Test Text</body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(templateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = new HandlerStub { Title = "Foo" } };

            var writer = new StringWriter();
            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);
            Assert.Equal(expected, writer.ToString().Trim());
        }

        public class HandlerStub
        {
            public string Title { get; set; }
        }
    }
}