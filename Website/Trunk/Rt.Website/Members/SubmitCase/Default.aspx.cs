using System;
using System.Net.Mail;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using log4net;
using Rt.Framework.Services;
using Rt.Website;

public partial class Members_SubmitCase_Default : Page
{
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void Submit_Click(object sender, EventArgs e)
	{
		if (!Page.IsValid)
			return;

		SubmitCasePanel.Visible = false;

		MembershipUser currentUser = null;
		MailMessage caseMessage = null;
		try
		{
			currentUser = Membership.GetUser();
			string userInformationLink = Global.ResolveUrl("~/Administrators/UserViewer/?Search=" + currentUser.ProviderUserKey);
			string ourMessage = string.Format("{0}{1}{1}{2}", Message.Text, Environment.NewLine, userInformationLink);

			caseMessage = new MailMessage(currentUser.Email, GlobalSettings.FogBugzSupportEmail, Subject.Text, ourMessage);
			caseMessage.IsBodyHtml = false;

			RtEngines.EmailEngine.SendEmail(caseMessage, false);

			CaseSubmittedPanel.Visible = true;
		}
		catch (Exception ex)
		{
			try
			{
				// Log the exception right away so that gets done no matter what happens in the next couple lines.
				_log.Fatal(ex);

				// Log which user entered the case
				if (currentUser != null)
					_log.FatalFormat("User {0} tried to enter a case but an exception occured.  (Exception was just logged.)",
					                 currentUser.UserName);
				else
					_log.Fatal(
						"A user tried to enter a case but an exception occured.  Current user is null for some reason.  (Exception was just logged.)");

				// Log the case information
				if (caseMessage != null)
					_log.InfoFormat("Here is the case the user tried to enter:{0}" +
					                "Title: {1}{0}" +
					                "Description: {2}", Environment.NewLine, caseMessage.Subject, caseMessage.Body);
				else
					_log.InfoFormat("Here is the case the user tried to enter:{0}" +
					                "Title: {1}{0}" +
					                "Description: {2}", Environment.NewLine, Subject.Text, Message.Text);

				CaseNotSubmittedPanel.Visible = true;
			}
			catch (Exception)
			{
				_log.FatalFormat(
					"A user tried to enter a case but an exception occurred.  Then when information about that exception was being logged, another exception occured.",
					ex);
			}
		}
	}
}