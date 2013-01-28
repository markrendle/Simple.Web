namespace Simple.Web.Razor.Tests
{
    using Xunit;

    public class TypeNameTranslatorTests
    {
        [Fact]
        public void TranslatesIEnumerableOfString()
        {
            Assert.Equal("IEnumerable`1[string]", TypeNameTranslator.CSharpNameToTypeName("IEnumerable<string>"));
        }
        
        [Fact]
        public void TranslatesIEnumerableOfIEnumerableOfString()
        {
            Assert.Equal("IEnumerable`1[IEnumerable`1[string]]", TypeNameTranslator.CSharpNameToTypeName("IEnumerable<IEnumerable<string>>"));
        }        

        [Fact]
        public void TranslatesIDictionary()
        {
            Assert.Equal("IDictionary`2[string,object]", TypeNameTranslator.CSharpNameToTypeName("IDictionary<string,object>"));
        }

        [Fact]
        public void TranslatesIEnumerableOfIDictionary()
        {
            Assert.Equal("IEnumerable`1[IDictionary`2[string,object]]", TypeNameTranslator.CSharpNameToTypeName("IEnumerable<IDictionary<string,object>>"));
        }
    }
}