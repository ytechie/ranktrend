using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Components.PayPal;

namespace Rt.Framework.Payments.PayPal
{
	public class PayPalManagement
	{
		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static SubscriptionActions GetSubscriptionAction(Subscription currentSubscription, Plan newPlan, PayPalInterval interval, Guid userId)
		{
			SubscriptionActions actions;

			actions = new SubscriptionActions();

			//They need a subscription if they're switching to a paid plan
			if (newPlan.Id == 1)
			{
				actions.Action = ActionTypes.Cancel;
			}
			else
			{
				actions.Action = ActionTypes.Modify;
				actions.NewSubscription = getNewSubscription(newPlan, interval.Units == IntervalUnits.Months, userId);
			}

			if (currentSubscription == null || currentSubscription.Plan == null || currentSubscription.Plan.Id == 1)
			{
				if (newPlan.Id == 1)
				{
					actions.Action = ActionTypes.NoChange;
				}
				else
				{
					//The modify method will create a new subscription when upgrading from the free account
					actions.Action = ActionTypes.StartPlan;
				}
			}

			return actions;
		}

		private static Subscription getNewSubscription(Plan plan, bool monthly, Guid userId)
		{
			Subscription subscription;

			subscription = new Subscription();
			subscription.Id = Guid.NewGuid();
			subscription.UserId = userId;
			subscription.Plan = plan;

			if (monthly)
			{
				subscription.PayPalInterval = "1 M";
				subscription.Cost = plan.MonthlyPrice;
			}
			else
			{
				subscription.PayPalInterval = "1 Y";
				subscription.Cost = plan.YearlyPrice;
			}

			return subscription;
		}
	}
}
