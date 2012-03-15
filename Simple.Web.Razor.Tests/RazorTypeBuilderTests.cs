using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.Razor.Tests
{
    using System.IO;
    using Xunit;

    public class RazorTypeBuilderTests
    {
        private const string TemplateText =
            @"@model Simple.Web.Razor.Tests.TestModel
<!DOCTYPE html><html><body>@Model.Text</body></html>";

        [Fact]
        public void GetsAType()
        {
            Type type;
            using (var reader = new StringReader(TemplateText))
            {
                type = new RazorTypeBuilder().CreateType(reader, typeof (TestModel));
            }
            Assert.NotNull(type);
        }

        [Fact]
        public void GetsModelTypeFromRazorMarkup()
        {
            Type type;
            using (var reader = new StringReader(TemplateText))
            {
                type = new RazorTypeBuilder().CreateType(reader);
            }
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof (TestModel), genericArguments[0]);
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
            Type type;
            using (var reader = new StringReader(templateText))
            {
                type = new RazorTypeBuilder().CreateType(reader);
            }
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof (IEnumerable<TestModel>), genericArguments[0]);
        }
    }

    public class TestModel
    {
        public string Text { get; set; }
    }
}
