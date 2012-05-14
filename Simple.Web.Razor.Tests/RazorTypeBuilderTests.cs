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
        private const string HandlerTemplateText =
            @"@handler Simple.Web.Razor.Tests.TestHandler
<!DOCTYPE html><html><body>@Handler.Text</body></html>";

        private const string ModelTemplateText =
            @"@model Simple.Web.Razor.Tests.TestModel
<!DOCTYPE html><html><body>@Model.Text</body></html>";

        [Fact]
        public void GetsATypeGivenAModel()
        {
            Type type;
            using (var reader = new StringReader(ModelTemplateText))
            {
                type = new RazorTypeBuilder().CreateType(reader, null, typeof (TestModel));
            }
            Assert.NotNull(type);
        }

        [Fact]
        public void GetsATypeGivenAHandler()
        {
            Type type;
            using (var reader = new StringReader(HandlerTemplateText))
            {
                type = new RazorTypeBuilder().CreateType(reader, typeof (TestHandler), null);
            }
            Assert.NotNull(type);
        }

        [Fact]
        public void GetsModelTypeFromRazorMarkup()
        {
            Type type;
            using (var reader = new StringReader(ModelTemplateText))
            {
                type = new RazorTypeBuilder().CreateType(reader);
            }
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof (TestModel), genericArguments[0]);
        }
        
        [Fact]
        public void GetsHandlerTypeFromRazorMarkup()
        {
            Type type;
            using (var reader = new StringReader(HandlerTemplateText))
            {
                type = new RazorTypeBuilder().CreateType(reader);
            }
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof (TestHandler), genericArguments[0]);
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

    public class TestHandler
    {
        public string Text { get; set; }
    }

    public class TestModel
    {
        public string Text { get; set; }
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
