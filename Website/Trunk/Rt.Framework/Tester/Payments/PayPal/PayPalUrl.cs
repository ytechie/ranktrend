using System;
using NUnit.Framework;
using Rt.Framework.Components;

namespace Rt.Framework.Payments.PayPal
{
	[TestFixture]
	public class PayPalUrl_Tester
	{
		[Test]
		public void BuildPayPalUrl()
		{
			Guid testGuid = Guid.NewGuid();
			Plan plan;
			string url;

			plan = new Plan();
			plan.MonthlyPrice = 1.99;
			plan.FriendlyName = "plan name";

			url = PayPalUrl.BuildPayPalUrl(plan, testGuid, false, true, "http://www.Google.com");

			Assert.IsTrue(url.EndsWith("cmd=_xclick-subscriptions&business=PayPal%40RankTrend.com&custom=" + testGuid.ToString() + "&item_name=plan+name&return=http%3a%2f%2fwww.Google.com&p3=1&a3=1.99&t3=M&src=1&no_note=1"));
		}

		[Test]
		public void BuildPayPalUrl2()
		{
			Guid testGuid = Guid.NewGuid();
			Plan plan;
			string url;

			plan = new Plan();
			plan.YearlyPrice = 19.99;
			plan.FriendlyName = "plan name";

			url = PayPalUrl.BuildPayPalUrl(plan, testGuid, false, false, "http://www.Google.com");

			Assert.IsTrue(url.EndsWith("cmd=_xclick-subscriptions&business=PayPal%40RankTrend.com&custom=" + testGuid.ToString() + "&item_name=plan+name&return=http%3a%2f%2fwww.Google.com&p3=1&a3=19.99&t3=Y&src=1&no_note=1"));
		}
	}
}
