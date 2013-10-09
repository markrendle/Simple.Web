using System.Collections.Generic;

namespace Simple.Web.Razor.Tests
{
    using Xunit;

    public class RazorTypeBuilderTests
    {
        private const string HandlerTemplateText =
            @"@handler Simple.Web.Razor.Tests.TestHandler
<!DOCTYPE html><html><body>@Handler.Text</body></html>";

        private const string ModelTemplateText =
            @"@model Simple.Web.Razor.Tests.TestModel
<!DOCTYPE html><html><body>@Model.Text</body></html>";

        [Fact]
        public void GetsModelTypeFromRazorMarkup()
        {
            var type = RazorTypeBuilderHelpers.CreateTypeFromText(ModelTemplateText);
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof(TestModel), genericArguments[0]);
        }

        [Fact]
        public void GetsHandlerTypeFromRazorMarkup()
        {
            var type = RazorTypeBuilderHelpers.CreateTypeFromText(HandlerTemplateText);
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof(TestHandler), genericArguments[0]);
        }

        [Fact]
        public void GetsComplexModelTypeFromRazorMarkup()
        {
            const string templateText =
                @"@model IEnumerable<Simple.Web.Razor.Tests.TestModel>
<!DOCTYPE html><html><body>@foreach (var m in @Model)
{
<p>@m.Text</p>
}
</body></html>";
            var type = RazorTypeBuilderHelpers.CreateTypeFromText(templateText);
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof(IEnumerable<TestModel>), genericArguments[0]);
        }


        [Fact]
        public void GetTypeFromViewWithExternalUsing()
        {
            const string templateText = @"
@using Simple.Web.Razor.Tests.ExternalDummyAssembly
<!DOCTYPE html>
<html>
	<head>
		<title>Test</title>
	</head>
	<body>
	    <p>@DummyHelper.DummyString</p>
	</body>
</html>
";
            var type = RazorTypeBuilderHelpers.CreateTypeFromText(templateText);
            Assert.NotNull(type);
        }

        [Fact]
        public void GetTypeFromViewWithExternalFullyQualifiedReference()
        {
            const string templateText = @"
<!DOCTYPE html>
<html>
<head>
    <title>Test</title>
</head>
<body>
    <p>@Simple.Web.Razor.Tests.ExternalDummyAssembly.DummyHelper.DummyString</p>
</body>
</html>";
            var type = RazorTypeBuilderHelpers.CreateTypeFromText(templateText);
            Assert.NotNull(type);
        }
    }

    public class TestHandler
    {
        public string Text { get; set; }
    }

    public class TestModel
    {
        public string Text { get; set; }
    }

    public class SuperTestModel : TestModel
    {

    }

    public class TestJustHandler
    {
        public string Text { get; set; }
    }

    public class TestJustModel
    {
        public string Text { get; set; }
    }

    public class TestModelWithHandler
    {
        public string Text { get; set; }
    }

    public class TestHandlerWithModel
    {
        public string Text { get; set; }
    }
}
