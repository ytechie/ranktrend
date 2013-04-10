using System;
using System.Reflection;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;

public partial class Forgot_Username_Default : Page
{
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			AdministratorEmail.Text = GlobalSettings.AdministrativeEmail;
		}
	}

	protected void LookupUsername_Click(object sender, EventArgs e)
	{
		ForgotUsername_Success.Visible = false;
		ForgotUsername_UsernameLookupFailure.Visible = false;

		string username = Membership.GetUserNameByEmail(EmailAddress.Text);
		if (string.IsNullOrEmpty(username))
		{
			ForgotUsername_UsernameLookupFailure.Visible = true;
			return;
		}

		MembershipUser membershipUser = Membership.GetUser(username);
		if (membershipUser == null)
		{
			ForgotUsername_UsernameLookupFailure.Visible = true;
			return;
		}

		ForgotUsername_Success.Visible = true;

		Database db = Global.GetDbConnection();
		var userInformation = db.ORManager.Get<UserInformation>(membershipUser.ProviderUserKey);

		// Save Verify User Email to email queue
		EmailTemplate emailTemplate = db.GetEmailTemplate(GlobalSettings.VerifyUserEmail);
		var emailMessage = new EmailMessage(emailTemplate);
		emailMessage.From = GlobalSettings.AdministrativeEmail;
		emailMessage.ApplyToUser(membershipUser, userInformation);
		db.SaveEmail(emailMessage);

		ThreadPool.QueueUserWorkItem(forceEmailEngineRun, membershipUser);
	}

	private static void forceEmailEngineRun(object state)
	{
		MembershipUser membershipUser = null;
		try
		{
			membershipUser = (MembershipUser) state;
		}
		catch (Exception ex)
		{
			_log.Warn("Failed to cast state to a MembershipUser object.", ex);
		}

		try
		{
			RtEngines.EmailEngine.RunFullCycle();
		}
		catch (Exception ex)
		{
			string user = membershipUser == null ? string.Empty : membershipUser.ProviderUserKey + " ";
			_log.Warn(
				string.Format(
					"When user {0}requested his/her username, an error occurred when attempting to force the email engine to run a cycle.",
					user), ex);
		}
	}
}