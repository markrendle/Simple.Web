namespace Simple.Web.Razor.Tests
{
    using Xunit;

    public class RazorViewsTests
    {
        public RazorViewsTests()
        {
            RazorViews.Initialize();
        }

        [Fact]
        public void FindsViewForModelType()
        {
            Assert.Equal(typeof(SimpleTemplateModelBase<TestJustModel>),
                new RazorViews().GetViewTypeForHandlerAndModelType(typeof(TestHandler), typeof(TestJustModel)).BaseType);
        }

        [Fact]
        public void FindsViewForHandlerType()
        {
            Assert.Equal(typeof (SimpleTemplateHandlerBase<TestJustHandler>),
                         new RazorViews().GetViewTypeForHandlerAndModelType(typeof (TestJustHandler), null).BaseType);
        }
        
        [Fact]
        public void FindsViewForHandlerAndModelType()
        {
            Assert.Equal(typeof (SimpleTemplateHandlerModelBase<TestHandler, TestModel>),
                         new RazorViews().GetViewTypeForHandlerAndModelType(typeof (TestHandler), typeof(TestModel)).BaseType);
        }
        
        [Fact]
        public void ThrowsViewNotFoundExceptionForInvalidModelType()
        {
            Assert.Throws<ViewNotFoundException>(() => new RazorViews().GetViewTypeForHandlerAndModelType(typeof(InvalidHandler), typeof(InvalidModel)));
        }
        
        [Fact]
        public void ThrowsAmbiguousViewExceptionForInvalidModelType()
        {
            Assert.Throws<AmbiguousViewException>(() => new RazorViews().GetViewTypeForHandlerAndModelType(typeof(InvalidHandler), typeof(AmbiguousModel)));
        }
    }

    public class InvalidModel
    {
        public string Text { get; set; }
    }

    public class InvalidHandler
    {
        public string Text { get; set; }
    }

    public class AmbiguousModel
    {
        public string Text { get; set; }
    }
}