using Xunit;

namespace Simple.Web.Tests.ApplicationTests
{
	public class ContentTypeTests
	{
		[Fact]
		public void FileWithASingleAcceptTypeReturnsThatType()
		{
			var type = Application.GetContentType("any", new[]{"test/type"});
			
			Assert.Equal("test/type", type);
		}
		
		[Fact]
		public void FileWithMultipleAcceptTypeReturnsFirstType()
		{
			var type = Application.GetContentType("any", new[]{"type/one", "type/two", "*/*"});
			
			Assert.Equal("type/one", type);
		}

		[Fact]
		public void FileWithNullAcceptTypesReturnsPlainText()
		{
			var type = Application.GetContentType("any", null);
			
			Assert.Equal("text/plain", type);
		}

		[Fact]
		public void FileWithEmptyAcceptTypesReturnsPlainText()
		{
			var type = Application.GetContentType("any", new string[]{});
			
			Assert.Equal("text/plain", type);
		}

		[Fact]
		public void FileWithWildcardAcceptTypeAndNoKnownExtensionReturnsPlainText()
		{
			var type = Application.GetContentType("any", new[]{"*/*"});
			
			Assert.Equal("text/plain", type);
		}

		[Fact]
		public void FileWithWildcardAcceptType_Jpg_ExtensionReturns_Jpeg()
		{
			var type = Application.GetContentType("any.jpg", new[]{"*/*"});
			
			Assert.Equal("image/jpeg", type);
		}
		[Fact]
		public void FileWithWildcardAcceptType_Jpeg_ExtensionReturns_Jpeg()
		{
			var type = Application.GetContentType("any.jpeg", new[]{"*/*"});
			
			Assert.Equal("image/jpeg", type);
		}
		[Fact]
		public void FileWithWildcardAcceptType_Png_ExtensionReturns_Png()
		{
			var type = Application.GetContentType("any.png", new[]{"*/*"});
			
			Assert.Equal("image/png", type);
		}
		[Fact]
		public void FileWithWildcardAcceptType_Gif_ExtensionReturns_gif()
		{
			var type = Application.GetContentType("any.gif", new[]{"*/*"});
			
			Assert.Equal("image/gif", type);
		}
		[Fact]
		public void FileWithWildcardAcceptType_Js_ExtensionReturns_JavaScript()
		{
			var type = Application.GetContentType("any.js", new[]{"*/*"});
			
			Assert.Equal("text/javascript", type);
		}
		[Fact]
		public void FileWithWildcardAcceptType_Javascript_ExtensionReturns_JavaScript()
		{
			var type = Application.GetContentType("any.JavaScript", new[]{"*/*"});
			
			Assert.Equal("text/javascript", type);
		}
	}
}
