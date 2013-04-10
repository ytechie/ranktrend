using System;
using System.Reflection;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;

public partial class Forgot_Password_Default : Page
{
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void PasswordRecovvery1_SendingMail(object sender, MailMessageEventArgs e)
	{
		Database db = Global.GetDbConnection();

		var msg = new EmailMessage();
		msg.From = GlobalSettings.AdministrativeEmail;
		msg.Subject = e.Message.Subject;
		msg.Html = e.Message.IsBodyHtml;
		msg.ToAddress = e.Message.To[0].Address;
		msg.Message = e.Message.Body;
		db.SaveEmail(msg);

		ThreadPool.QueueUserWorkItem(forceEmailEngineRun, msg);

		e.Cancel = true; // Cancel automatic message since we handle it through out email queue.
	}

	private static void forceEmailEngineRun(object state)
	{
		EmailMessage msg = null;
		try
		{
			msg = (EmailMessage) state;
		}
		catch (Exception ex)
		{
			_log.Warn("Failed to cast state into an EmailMessage object.", ex);
		}

		try
		{
			RtEngines.EmailEngine.RunFullCycle();
		}
		catch (Exception ex)
		{
			string user = msg == null ? string.Empty : msg.ToAddress + " ";
			_log.Warn(
				string.Format(
					"When user {0}requested his/her password, an error occurred when attempting to force the email engine to run a cycle.",
					user), ex);
		}
	}
}