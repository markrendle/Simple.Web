namespace Simple.Web.Razor.Tests
{
    using System;
    using System.IO;

    using Simple.Web.TestHelpers;

    using Xunit;

    public class RazorSectionsTests
    {
        [Fact]
        public void RenderSectionWithinPage()
        {
            const string TemplateText = @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayout.cshtml"";
}
<p>@Model.Text</p>
@section Two {<p>I am in section two.</p>}
@RenderSection(""Two"", required:true)";

            const string Expected =
                @"<!DOCTYPE html><html><head><title>Simple Layout Page</title></head><body><p>Test Text</p><p>I am in section two.</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        [Fact]
        public void RenderTwoSection()
        {
            const string TemplateText = @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayoutWithSections.cshtml"";
}
<span>@Model.Text</span>
@section One {<p>I am in section one.</p>}
@section Two {<p>I am in section two.</p>}";

            const string Expected =
                @"<!DOCTYPE html><html><head><title>Sections Layout Page</title></head><body><h1>Section One</h1><p>I am in section one.</p><p><span>Test Text</span></p><h1>Section Two</h1><p>I am in section two.</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);
            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        [Fact]
        public void RenderWithOptionalSection()
        {
            const string TemplateText = @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayoutWithSections.cshtml"";
}
<span>@Model.Text</span>
@section Two {<p>I am in section two.</p>}";

            const string Expected =
                @"<!DOCTYPE html><html><head><title>Sections Layout Page</title></head><body><h1>Section One</h1><p><span>Test Text</span></p><h1>Section Two</h1><p>I am in section two.</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        [Fact]
        public void RenderWithSectionDuplicated()
        {
            const string TemplateText = @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayoutWithSectionDuplicated.cshtml"";
}
<span>@Model.Text</span>
@section Two {<p>I am in section two.</p>}";

            const string Expected =
                @"<!DOCTYPE html><html><head><title>Sections Layout Page</title></head><body><h1>Section Two - Once</h1><p>I am in section two.</p><p><span>Test Text</span></p><h1>Section Two - Twice</h1><p>I am in section two.</p></body></html>";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            RazorHtmlMediaTypeHandler.RenderView(output, writer, type);

            Assert.Equal(Expected, HtmlComparison.Cleanse(writer.ToString()));
        }

        [Fact]
        public void RenderWithoutNonOptionalErrors()
        {
            const string TemplateText = @"@model Simple.Web.Razor.Tests.TestModel
@{
    Layout = ""~/Views/Layouts/SimpleLayoutWithSections.cshtml"";
}
<span>@Model.Text</span>
@section One {<p>I am in section one.</p>}";

            var type = RazorTypeBuilderHelpers.CreateTypeFromText(TemplateText);

            var output = new MockHandler { Model = new TestModel { Text = "Test Text" }, Handler = null };
            var writer = new StringWriter();

            Assert.Throws<ArgumentException>(() => RazorHtmlMediaTypeHandler.RenderView(output, writer, type));
        }
    }
}