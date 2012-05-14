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
                new RazorViews().GetViewType(typeof(TestHandler), typeof(TestJustModel)).BaseType);
        }

        [Fact]
        public void FindsViewForDerivedModelType()
        {
            Assert.Equal(typeof(SimpleTemplateModelBase<TestJustModel>),
                new RazorViews().GetViewType(typeof(TestHandler), typeof(SuperTestJustModel)).BaseType);
        }

        [Fact]
        public void FindsViewForInterfaceModelType()
        {
            Assert.Equal(typeof(SimpleTemplateModelBase<ITestInterfaceModel>),
                new RazorViews().GetViewType(typeof(TestHandler), typeof(TestInterfaceModel)).BaseType);
        }

        [Fact]
        public void FindsViewForHandlerType()
        {
            Assert.Equal(typeof (SimpleTemplateHandlerBase<TestJustHandler>),
                         new RazorViews().GetViewType(typeof (TestJustHandler), null).BaseType);
        }
        
        [Fact]
        public void FindsViewForDerivedHandlerType()
        {
            Assert.Equal(typeof (SimpleTemplateHandlerBase<TestJustHandler>),
                         new RazorViews().GetViewType(typeof (SuperTestJustHandler), null).BaseType);
        }
        
        [Fact]
        public void FindsViewForInterfaceHandlerType()
        {
            Assert.Equal(typeof (SimpleTemplateHandlerBase<ITestInterfaceHandler>),
                         new RazorViews().GetViewType(typeof (TestInterfaceHandler), null).BaseType);
        }
        
        [Fact]
        public void FindsViewForHandlerAndModelType()
        {
            Assert.Equal(typeof (SimpleTemplateHandlerModelBase<TestHandler, TestModel>),
                         new RazorViews().GetViewType(typeof (TestHandler), typeof(TestModel)).BaseType);
        }
        
        [Fact]
        public void ThrowsViewNotFoundExceptionForInvalidModelType()
        {
            Assert.Throws<ViewNotFoundException>(() => new RazorViews().GetViewType(typeof(InvalidHandler), typeof(InvalidModel)));
        }
        
        [Fact]
        public void ThrowsAmbiguousViewExceptionForInvalidModelType()
        {
            Assert.Throws<AmbiguousViewException>(() => new RazorViews().GetViewType(typeof(InvalidHandler), typeof(AmbiguousModel)));
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

    public class SuperTestJustModel : TestJustModel
    {
        
    }
    
    public class SuperTestJustHandler : TestJustHandler
    {
        
    }

    public interface ITestInterfaceHandler
    {
        string Text { get; set; }
    }

    public class TestInterfaceHandler : ITestInterfaceHandler
    {
        public string Text { get; set; }
    }
    
    public interface ITestInterfaceModel
    {
        string Text { get; set; }
    }

    public class TestInterfaceModel : ITestInterfaceModel
    {
        public string Text { get; set; }
    }
}