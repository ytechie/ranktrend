using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Rt.Framework.Components.PayPal;

namespace Rt.Framework.Payments.PayPal
{
	[TestFixture]
	public class IpnPostHandler_Tester
	{
		private IpnPostHandler iph;
		private DateTime dbNow;

		[SetUp]
		public void SetUp()
		{
			dbNow = DateTime.Now;
		}

		[Test]
		public void ModifySubscription()
		{
			NameValueCollection postData;
			Subscription existingSubscription;

			postData = new NameValueCollection();
			postData.Add(IpnPostHandler.PP_TRANSACTION_TYPE, IpnPostHandler.PP_TRANSACTION_TYPE_MODIFY);
			postData.Add("amount3", "9.99");
			postData.Add("period3", "1 Y");

			existingSubscription = new Subscription();
			existingSubscription.Cost = 1.99;
			existingSubscription.PayPalInterval = "1 M";

			iph = new IpnPostHandler(postData, existingSubscription);
			iph.Process(dbNow);

			Assert.AreEqual(existingSubscription, iph.SaveSubscription);
			Assert.AreEqual(existingSubscription.Id, iph.SaveSubscription.Id);
			Assert.AreEqual(9.99, iph.SaveSubscription.Cost);
			Assert.AreEqual("1 Y", iph.SaveSubscription.PayPalInterval);
		}

		[Test]
		public void ProcessPayment()
		{
			NameValueCollection postData;
			Subscription existingSubscription;

			postData = new NameValueCollection();
			postData.Add(IpnPostHandler.PP_TRANSACTION_TYPE, IpnPostHandler.PP_TRANSACTION_TYPE_PAYMENT);

			existingSubscription = new Subscription();
			existingSubscription.Cost = 1.99;
			existingSubscription.PayPalInterval = "1 M";

			iph = new IpnPostHandler(postData, existingSubscription);
			iph.Process(dbNow);

			//Make sure we're getting back the same subscription
			Assert.AreEqual(existingSubscription, iph.SaveSubscription);
			Assert.AreEqual(existingSubscription.Id, iph.SaveSubscription.Id);

			//Make sure these didn't change
			Assert.AreEqual(1.99, iph.SaveSubscription.Cost);
			Assert.AreEqual("1 M", iph.SaveSubscription.PayPalInterval);

			//Make sure the interval was refreshed
			Assert.AreEqual(dbNow.AddMonths(1), iph.SaveSubscription.EndTime);
		}
	}
}
