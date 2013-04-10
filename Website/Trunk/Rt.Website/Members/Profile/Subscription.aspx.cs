using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using log4net;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Components.PayPal;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Payments.PayPal;
using Rt.Website;

public partial class Members_Profile_Subscription : Page
{
	/// <summary>
	///		Declare and create our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Database _db;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();
		_userId = (Guid) Membership.GetUser().ProviderUserKey;

		cmdChangePlan.Click += cmdChangePlan_Click;
	}

	private void cmdChangePlan_Click(object sender, EventArgs e)
	{
		Subscription currentSubscription;
		SubscriptionActions actionInfo;
		Plan selectedPlan;
		bool payMonthly;
		PayPalInterval interval;
		string redirectUrl = "";

		currentSubscription = getCurrentSubscription();
		selectedPlan = getSelectedPlan(lstPlans.SelectedValue, out payMonthly);
		interval = new PayPalInterval(1, payMonthly ? IntervalUnits.Months : IntervalUnits.Years);

		actionInfo = PayPalManagement.GetSubscriptionAction(currentSubscription, selectedPlan, interval, _userId);

		if (actionInfo.Action == ActionTypes.NoChange)
			return;
		else if (actionInfo.Action == ActionTypes.Cancel)
			//Todo: build a cancellation URL
			throw new NotImplementedException();
		else if (actionInfo.Action == ActionTypes.StartPlan)
			redirectUrl = PayPalUrl.BuildPayPalUrl(selectedPlan, actionInfo.NewSubscription.Id, false, payMonthly, getReturnUrl());
		else if (actionInfo.Action == ActionTypes.Modify)
			redirectUrl = PayPalUrl.BuildPayPalUrl(selectedPlan, actionInfo.NewSubscription.Id, true, payMonthly, getReturnUrl());

		if (actionInfo.NewSubscription != null)
			_db.ORManager.Save(actionInfo.NewSubscription);

		if (redirectUrl.Length > 0)
			Response.Redirect(redirectUrl);
	}

	private static string getReturnUrl()
	{
		return Global.ResolveUrl("~/Members/");
	}

	private Subscription getCurrentSubscription()
	{
		ICriteria criteria;
		IList<Subscription> subscriptions;

		criteria = _db.ORManager.Session.CreateCriteria(typeof (Subscription));
		criteria.Add(Expression.Eq("UserId", _userId));
		criteria.Add(Expression.IsNotNull("EndTime"));
		criteria.AddOrder(Order.Desc("EndTime"));

		subscriptions = criteria.List<Subscription>();

		if (subscriptions.Count == 0)
			return null;
		else
			return subscriptions[0];
	}

	private Plan getSelectedPlan(string planCode, out bool monthly)
	{
		int selectedPlanId;
		Plan selectedPlan;

		selectedPlanId = int.Parse(planCode.Substring(0, 1));
		selectedPlan = _db.ORManager.Get<Plan>(selectedPlanId);

		if (selectedPlanId == 1)
			monthly = true;
		else
			monthly = planCode.EndsWith("M");

		return selectedPlan;
	}
}