using System.Collections.Generic;// ReSharper disable ReturnTypeCanBeEnumerable.Local
using NUnit.Framework;
using Simple.Web.Owin;

namespace SelfHost.Unit.Tests
{
	[TestFixture]
	public class QueryStringTests
	{
		[Test]
		public void Empty_string_is_empty_dictionary()
		{
			Assert.That(
				"".ToQueryDictionary(),
				Is.EquivalentTo(EmptyIDictionary()));
		}

		[Test]
		public void A_key_and_value_are_separated_by_equals_sign()
		{
			Assert.That(
				"key=value".ToQueryDictionary(),
				Is.EquivalentTo(ThisDictionary("key","value")));
		}

		[Test]
		public void Multiple_keys_and_values_are_separated_by_and_sign()
		{
			Assert.That(
				"key1=value1&key2=value2".ToQueryDictionary(),
				Is.EquivalentTo(ThisDictionary(
				"key1","value1",
				"key2","value2")));
		}
		
		[Test]
		public void Multiple_equals_signs_in_values_are_ignored()
		{
			Assert.That(
				"key1=val=ue1&key2=value=2".ToQueryDictionary(),
				Is.EquivalentTo(ThisDictionary(
				"key1","val=ue1",
				"key2","value=2")));
		}
		
		[Test]
		public void Missing_values_are_empty_strings()
		{
			Assert.That(
				"key1=value1&key2&key3=value3".ToQueryDictionary(),
				Is.EquivalentTo(ThisDictionary(
				"key1", "value1",
				"key2", "",
				"key3", "value3")));
		}
		
		[Test]
		public void Empty_key_value_pairs_are_ignored()
		{
			Assert.That(
				"key1=value1&&key3=value3".ToQueryDictionary(),
				Is.EquivalentTo(ThisDictionary(
				"key1", "value1",
				"key3", "value3")));
		}

		IDictionary<string,string> ThisDictionary(params string[] strings)
		{
			string k = null;
			var d = new Dictionary<string,string>();
			foreach (var s in strings)
			{
				if (k == null) k = s;
				else
				{
					d.Add(k, s);
					k = null;
				}
			}
			return d;
		}
		IDictionary<string,string> EmptyIDictionary()
		{
			return new Dictionary<string,string>();
		}
	}
}
