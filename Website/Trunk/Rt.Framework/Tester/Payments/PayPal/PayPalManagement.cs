using System;
using NUnit.Framework;
using Rt.Framework.Components;
using Rt.Framework.Components.PayPal;

namespace Rt.Framework.Payments.PayPal
{
	[TestFixture]
	public class PayPalManagement_Tester
	{
		private Plan freePlan;
		private Plan paidPlan;
		private Plan paidPlan2;

		[SetUp]
		public void SetUp()
		{
			freePlan = new Plan();
			freePlan.FriendlyName = "Free Plan";
			freePlan.Id = 1;
			freePlan.MonthlyPrice = 0;

			paidPlan = new Plan();
			paidPlan.FriendlyName = "Paid Plan";
			paidPlan.Id = 2;
			paidPlan.MonthlyPrice = 1.99;
			paidPlan.YearlyPrice = 24.99;

			paidPlan2 = new Plan();
			paidPlan2.FriendlyName = "Paid Plan 2";
			paidPlan2.Id = 3;
			paidPlan2.MonthlyPrice = 19.99;
			paidPlan2.YearlyPrice = 199.99;
		}

		/// <summary>
		///		Verify that switching from free to free gives back no action
		/// </summary>
		[Test]
		public void GetSubscriptionAction()
		{
			Subscription sub;
			Plan newPlan;
			PayPalInterval interval;
			Guid userId;
			SubscriptionActions actions;

			sub = new Subscription();
			sub.Plan = freePlan;

			newPlan = freePlan;

			interval = null;

			userId = Guid.NewGuid();

			actions = PayPalManagement.GetSubscriptionAction(sub, newPlan, interval, userId);
			Assert.AreEqual(ActionTypes.NoChange, actions.Action);
			Assert.AreEqual(null, actions.NewSubscription);
		}

		/// <summary>
		///		Check going from free to paid
		/// </summary>
		[Test]
		public void GetSubscriptionAction2()
		{
			Subscription sub;
			Plan newPlan;
			PayPalInterval interval;
			Guid userId;
			SubscriptionActions actions;

			sub = new Subscription();
			sub.Plan = freePlan;

			newPlan = paidPlan;

			interval = new PayPalInterval(1, IntervalUnits.Months);

			userId = Guid.NewGuid();

			actions = PayPalManagement.GetSubscriptionAction(sub, newPlan, interval, userId);
			Assert.AreEqual(ActionTypes.StartPlan, actions.Action);
			Assert.AreEqual(1.99, actions.NewSubscription.Cost);
			Assert.AreEqual(null, actions.NewSubscription.EndTime);
			Assert.IsTrue(actions.NewSubscription.Id != Guid.Empty);
			Assert.AreEqual("1 M", actions.NewSubscription.PayPalInterval);
			Assert.AreEqual(newPlan, actions.NewSubscription.Plan);
			Assert.AreEqual(userId, actions.NewSubscription.UserId);
		}

		/// <summary>
		///		Check switching between plans
		/// </summary>
		[Test]
		public void GetSubscriptionAction3()
		{
			Subscription sub;
			Plan newPlan;
			PayPalInterval interval;
			Guid userId;
			SubscriptionActions actions;

			sub = new Subscription();
			sub.Plan = paidPlan;

			newPlan = paidPlan2;

			interval = new PayPalInterval(1, IntervalUnits.Years);

			userId = Guid.NewGuid();

			actions = PayPalManagement.GetSubscriptionAction(sub, newPlan, interval, userId);

			Assert.AreEqual(ActionTypes.Modify, actions.Action);
			Assert.AreEqual(199.99, actions.NewSubscription.Cost);
			Assert.AreEqual(null, actions.NewSubscription.EndTime);
			Assert.IsTrue(actions.NewSubscription.Id != Guid.Empty);
			Assert.AreEqual("1 Y", actions.NewSubscription.PayPalInterval);
			Assert.AreEqual(newPlan, actions.NewSubscription.Plan);
			Assert.AreEqual(userId, actions.NewSubscription.UserId);
		}

		/// <summary>
		///		Check downgrading to the free plan
		/// </summary>
		[Test]
		public void GetSubscriptionAction4()
		{
			Subscription sub;
			Plan newPlan;
			PayPalInterval interval;
			Guid userId;
			SubscriptionActions actions;

			sub = new Subscription();
			sub.Plan = paidPlan;

			newPlan = freePlan;

			interval = null;

			userId = Guid.NewGuid();

			actions = PayPalManagement.GetSubscriptionAction(sub, newPlan, interval, userId);

			Assert.AreEqual(ActionTypes.Cancel, actions.Action);
			Assert.AreEqual(null, actions.NewSubscription);
		}
	}
}
