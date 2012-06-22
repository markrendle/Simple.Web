using NUnit.Framework;

namespace Simple.Web.Owin.Tests
{
	[TestFixture]
	public class HeaderDictionaryTests
	{
		[Test]
		public void HeaderDictionaryWithNoKeysCanIncludeAKey()
		{
			var subject = new HeaderDictionary();
			subject.Include("k", "v");

			Assert.That(subject["k"], Is.EquivalentTo(new[]{"v"}));
		}

		[Test]
		public void HeaderDictionaryWithExistingKeyCanIncludeAKey()
		{
			var subject = new HeaderDictionary();
			subject.Include("k", "v1");
			subject.Include("k", "v2");

			Assert.That(subject["k"], Is.EquivalentTo(new[]{"v1", "v2"}));
		}

		[Test]
		public void HeaderDictionaryWithExistingKeyCanResetAKeysValue()
		{
			var subject = new HeaderDictionary();
			subject.Include("k", "v1");
			subject.Include("k", "v2");
			subject.Set("k", "v0");

			Assert.That(subject["k"], Is.EquivalentTo(new[]{"v0"}));
		}
	}
}
