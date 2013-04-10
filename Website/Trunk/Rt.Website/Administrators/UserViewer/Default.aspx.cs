using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Administrators_UserViewer_Default : Page
{
	private const string PARAM_SEARCH = "Search";
	private const string REGEXPAT_GUID = "[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}";
	private static readonly Regex REGEX_GUID = new Regex(REGEXPAT_GUID, RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private MembershipUser user;

	protected void Page_Load(object sender, EventArgs e)
	{
		string searchString = Request.QueryString[PARAM_SEARCH];

		if (searchString == null)
			searchString = txtUser.Text;

		cmdLogon.Click += cmdLogon_Click;

		if (searchString != null)
		{
			txtUser.Text = searchString;
			user = getUser(searchString);
		}

		if (!Page.IsPostBack && user != null)
			setUser(user);
	}

	private void cmdLogon_Click(object sender, EventArgs e)
	{
		if (user != null)
			switchUser(user.UserName);
	}

	protected void Search_Click(object sender, EventArgs e)
	{
		if (!Page.IsValid)
			return;

		MembershipUser user = getUser(txtUser.Text);
		setUser(user);
	}

	private void setUser(MembershipUser user)
	{
		if (user != null)
		{
			UserNotFoundPanel.Visible = false;
			UserInformationPanel.Visible = true;

			Database db = Global.GetDbConnection();
			DataSet ds = db.GetUserStatusReport(user.ProviderUserKey);

			UserInformationDetails1.DataSource = ds.Tables[0];
			UserInformationDetails1.DataBind();

			UserInformationDetails2.DataSource = ds.Tables[1];
			UserInformationDetails2.DataBind();

			UserInformationDetails3.DataSource = ds.Tables[2];
			UserInformationDetails3.DataBind();
		}
		else
		{
			UserNotFoundPanel.Visible = true;
			UserInformationPanel.Visible = false;
		}
	}

	private MembershipUser getUser(string searchString)
	{
		MembershipUser user = null;
		if (REGEX_GUID.IsMatch(searchString))
		{
			var userKey = new Guid(searchString);
			user = Membership.GetUser(userKey);
		}
		else
		{
			string username = Membership.GetUserNameByEmail(searchString);

			if (username == null)
				username = searchString;

			user = Membership.GetUser(username);
		}

		return user;
	}

	private void switchUser(string userName)
	{
		FormsAuthentication.SetAuthCookie(userName, false);
		Response.Redirect("~/");
	}
}