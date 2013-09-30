namespace Simple.Web.Razor.Tests
{
    using System.IO;

    using Simple.Web.TestHelpers;

    using Xunit;

    public class RazorLayoutTests
    {
        [Fact]
        public void RendersSimpleLayout()
        {
            const string TemplateText =
                @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayout.cshtml"";
}<p>@Model.Text</p>";

            const string Expected = @"<!DOCTYPE html><html><head><title>Simple Layout Page</title></head><body><p>Test Text</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        [Fact]
        public void RendersSimpleLayoutWithTitle()
        {
            const string TemplateText =
                @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayoutWithTitle.cshtml"";
    ViewBag.Title = ""Custom Layout Title"";
}<p>@Model.Text</p>";

            const string Expected = @"<!DOCTYPE html><html><head><title>Custom Layout Title</title></head><body><p>Test Text</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        [Fact]
        public void RendersSimpleLayoutWithReferencedTitle()
        {
            const string TemplateText =
                @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayoutWithTitle.cshtml"";
    ViewBag.Title = @Handler.Title;
}<p>@Model.Text</p>";

            const string Expected = @"<!DOCTYPE html><html><head><title>Foo</title></head><body><p>Test Text</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = new HandlerStub { Title = "Foo" } };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        public class HandlerStub
        {
            public string Title { get; set; }
        }
    }
}
