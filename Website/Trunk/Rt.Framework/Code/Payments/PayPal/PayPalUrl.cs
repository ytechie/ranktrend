using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Components;
using YTech.General.Web;

namespace Rt.Framework.Payments.PayPal
{
	public static class PayPalUrl
	{
#if(DEBUG)
		public const string PAYPAL_BASE_URL = "https://www.sandbox.paypal.com/us/cgi-bin/webscr?";
		public const string PAYPAL_EMAIL = "PayPal@RankTrend.com";
#else
		public const string PAYPAL_BASE_URL = "https://www.paypal.com/cgi-bin/webscr?";
		public const string PAYPAL_EMAIL = "PayPal@RankTrend.com";
#endif

		public static string BuildPayPalUrl(Plan newPlan, Guid subscriptionId, bool modification, bool monthly, string returnUrl)
		{
			UrlBuilder url;
			url = new UrlBuilder(PAYPAL_BASE_URL);

			url.Parameters.AddParameter("cmd", "_xclick-subscriptions");
			url.Parameters.AddParameter("business", PAYPAL_EMAIL);

			if (subscriptionId != Guid.Empty)
				url.Parameters.AddParameter("custom", subscriptionId);

			//Add the subscription description
			url.Parameters.AddParameter("item_name", newPlan.FriendlyName);
			//Set the return URL
			url.Parameters.AddParameter("return", returnUrl);

			//The period is one unit (1 month or 1 year)
			url.Parameters.AddParameter("p3", 1);

			if (monthly)
			{
				url.Parameters.AddParameter("a3", newPlan.MonthlyPrice);
				url.Parameters.AddParameter("t3", "M");
			}
			else
			{
				url.Parameters.AddParameter("a3", newPlan.YearlyPrice);
				url.Parameters.AddParameter("t3", "Y");
			}

			//Set the subscription to recurring
			url.Parameters.AddParameter("src", 1);

			//Check if we should only allow modifying
			if (modification)
				url.Parameters.AddParameter("modify", 2);

			//Don't allow notes (they're not available anyway)
			url.Parameters.AddParameter("no_note", 1);

			return url.ToString();
		}
	}
}
