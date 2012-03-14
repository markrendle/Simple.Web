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
            Assert.Equal(typeof(SimpleTemplateBase<TestModel>), new RazorViews().GetViewTypeForModelType(typeof(TestModel)).BaseType);
        }
        
        [Fact]
        public void ThrowsViewNotFoundExceptionForInvalidModelType()
        {
            Assert.Throws<ViewNotFoundException>(() => new RazorViews().GetViewTypeForModelType(typeof(InvalidModel)));
        }
        
        [Fact]
        public void ThrowsAmbiguousViewExceptionForInvalidModelType()
        {
            Assert.Throws<AmbiguousViewException>(() => new RazorViews().GetViewTypeForModelType(typeof(AmbiguousModel)));
        }
    }

    public class InvalidModel
    {
        public string Text { get; set; }
    }

    public class AmbiguousModel
    {
        public string Text { get; set; }
    }
}