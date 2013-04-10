using System.Collections.Generic;
using NUnit.Framework;

namespace Rt.Framework.Web
{
	[TestFixture]
	public class LeadUrlRewriter_Tester
	{
		private LeadUrlRewriter lur;

		[SetUp]
		public void SetUp()
		{
			Dictionary<string, string> map;

			map = new Dictionary<string, string>();
			map.Add("1", "one");
			map.Add("2", "two");
			map.Add("3", "three");

			lur = new LeadUrlRewriter(map);
		}

		[Test]
		public void Add()
		{
			Dictionary<string, string> map;

			lur.Add("4", "four");
			map = lur.RewriteList;

			Assert.AreEqual(4, map.Count);
			Assert.AreEqual("four", map["4"]);
		}

		[Test]
		public void ChangeKey()
		{
			Dictionary<string, string> map;

			lur.ChangeKey("2", "4");
			map = lur.RewriteList;

			Assert.AreEqual(3, map.Count);
			Assert.AreEqual("one", map["1"]);
			Assert.AreEqual("two", map["4"]);
			Assert.AreEqual("three", map["3"]);
		}

		[Test]
		public void DeleteKey()
		{
			Dictionary<string, string> map;

			lur.DeleteKey("2");
			map = lur.RewriteList;

			Assert.AreEqual(2, map.Count);
			Assert.AreEqual("one", map["1"]);
			Assert.AreEqual("three", map["3"]);
		}

		[Test]
		public void ChangeRewritePath()
		{
			Dictionary<string, string> map;

			lur.ChangeRewritePath("2", "new!");
			map = lur.RewriteList;

			Assert.AreEqual(3, map.Count);
			Assert.AreEqual("one", map["1"]);
			Assert.AreEqual("new!", map["2"]);
			Assert.AreEqual("three", map["3"]);
		}
	}
}
