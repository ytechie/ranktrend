using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI;
using log4net;
using Rt.Framework.Applications;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web;

public partial class Verify_User_Default : Page
{
	private const string PARAM_KEY = "Key";

	private const string REGEXPAT_GUID = "[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}";

	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	private static readonly Regex REGEX_GUID = new Regex(REGEXPAT_GUID, RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private string Key
	{
		get
		{
			object key = Request.QueryString[PARAM_KEY];
			return key == null ? string.Empty : (string) key;
		}
	}

	public static string GetLoadUrl(string userId)
	{
		var url = new UrlBuilder("~/Verify-User/");
		url.Parameters.AddParameter(PARAM_KEY, userId);
		return url.ToString();
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		UserNotApprovedPanel.Visible = false;
		UserApprovedPanel.Visible = false;

		string key = Key;
		MembershipUser user = null;
		if (REGEX_GUID.IsMatch(key))
		{
			var userKey = new Guid(Key);
			user = Membership.GetUser(userKey);
		}

		if (user != null)
		{
			user.IsApproved = true;
			Membership.UpdateUser(user);
			if (!Roles.IsUserInRole(user.UserName, "Users")) Roles.AddUserToRole(user.UserName, "Users");
			UserName.Text = user.UserName;
			UserApprovedPanel.Visible = true;


			try
			{
				Database db = Global.GetDbConnection();
				if (GlobalSettings.WelcomeEmail != null && GlobalSettings.AdministrativeEmail != null)
					EmailQueue.EnqueueEmail(Global.GetDbConnection(), db.GetEmailTemplate(GlobalSettings.WelcomeEmail.Value), user,
					                        GlobalSettings.AdministrativeEmail);
			}
			catch (Exception ex)
			{
				_log.Fatal(
					string.Format("Error trying to send user {0} ({1}) the welcome email.", user.UserName, user.ProviderUserKey), ex);
			}
		}
		else
			UserNotApprovedPanel.Visible = true;
	}
}