using System;
using System.Web.Security;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Login_Default : Page
{
	protected string ReturnUrl
	{
		get
		{
			string returnUrl = Request.QueryString["ReturnUrl"];
			return returnUrl == null ? ResolveUrl("../Members/") : Server.UrlDecode(returnUrl);
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Membership.GetUser() != null)
			Response.Redirect("~/Members/");

		if (!Page.IsPostBack)
		{
			string returnUrl = ReturnUrl;

			if (returnUrl.IndexOf("/Members/") >= 0 || returnUrl.IndexOf("/Administrators/") >= 0)
				loginControl.DestinationPageUrl = returnUrl;
			else
				loginControl.DestinationPageUrl = ResolveUrl("../Members/");
		}
	}

	protected void loginControl_LoggedIn(object sender, EventArgs e)
	{
		MembershipUser user = Membership.GetUser(loginControl.UserName);
		if (user != null && !Roles.IsUserInRole(user.UserName, "Users")) Roles.AddUserToRole(user.UserName, "Users");
		checkTosAgreement(user);
	}

	private void checkTosAgreement(MembershipUser user)
	{
		Database db = Global.GetDbConnection();
		LegalNoticeVersion tosVersion = db.GetLatestLegalNoticeVersion(GlobalSettings.SignUpTermsOfService);
		if (!db.HasAgreedToNotice((int) tosVersion.Id, user.ProviderUserKey))
			Response.Redirect("~/Members/TermsOfService/default.aspx");
	}
}