using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MichaelBrumm.Win32;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;
using YTech.General.Web.JavaScript;

public partial class Members_Profile_Default : Page
{
	private const string TOKEN_CANCELLATION_REASON = "{CANCELLATION_REASON}";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initUserInformation();
			populateTimeZoneList();

			JavaScriptBlock.ConfirmClick(CancelAccount,
			                             @"If you cancel your account, you will no longer be able to log in to our system and all of the data gathered for your account will be deleted.\nAre you sure you want to cancel your account? (OK = Yes, Cancel = No)");
		}

		cmdUpdateTimezone.Click += cmdUpdateTimezone_Click;
	}

	protected void CancelAccount_Click(object sender, EventArgs e)
	{
		MembershipUser currUser = Membership.GetUser();

		Database db = Global.GetDbConnection();
		var email = new EmailMessage(db.GetEmailTemplate(9));
		email.From = "Support@RankTrend.com"; // Placeholder, will be replaced by Email Engine.
		email.ReplaceGeneralTokens();
		email.ApplyToUser(currUser, db.ORManager.Get<UserInformation>(currUser.ProviderUserKey));
		email.ToAddress = "Jason@Young-Technologies.com";
		email.ReplaceInMessage(TOKEN_CANCELLATION_REASON, AccountCancellationReason.Text);
		RtEngines.EmailEngine.SendEmail(email.GetMailMessage());

		currUser.IsApproved = false;
		Membership.UpdateUser(currUser);

		// TODO: Replace this with a better way of logging the user out if there is one.
		HttpCookie authCookie = Request.Cookies[".ASPXAUTH"];
		authCookie.Expires = DateTime.Now.AddYears(-1);
		Response.Cookies.Set(authCookie);

		redirectHome();
	}

	private void cmdUpdateTimezone_Click(object sender, EventArgs e)
	{
		Profile.TimeZoneIndex = int.Parse(ddlTimezone.SelectedValue);
		Profile.Save();
		redirectHome();
	}

	private void populateTimeZoneList()
	{
		Win32TimeZone[] timezones;
		ListItem newItem;

		timezones = TimeZones.GetTimeZones();

		foreach (Win32TimeZone currTimeZone in timezones)
		{
			newItem = new ListItem(currTimeZone.DisplayName, currTimeZone.Index.ToString());
			ddlTimezone.Items.Add(newItem);

			//Select the timezone from the profile
			if (currTimeZone.Index == Profile.TimeZoneIndex)
				newItem.Selected = true;
		}
	}

	protected void SaveUserInformationButton_Click(object sender, EventArgs e)
	{
		if (Page.IsValid)
		{
			MembershipUser membershipUser = Membership.GetUser();
			Database db = Global.GetDbConnection();

			var userInformation = db.ORManager.Get<UserInformation>(membershipUser.ProviderUserKey);
			userInformation.FirstName = FirstName.Text;
			userInformation.LastName = LastName.Text;

			db.ORManager.SaveOrUpdate(userInformation);

			redirectHome();
		}
	}

	protected void CancelUserInformationButton_Click(object sender, EventArgs e)
	{
		redirectHome();
	}

	private void initUserInformation()
	{
		Database db = Global.GetDbConnection();
		UserInformation userInformation;
		userInformation = db.ORManager.Get<UserInformation>(Membership.GetUser().ProviderUserKey);

		if (userInformation != null)
		{
			FirstName.Text = userInformation.FirstName;
			LastName.Text = userInformation.LastName;
		}
	}

	private void redirectHome()
	{
		Response.Redirect("~/Members/");
	}
}