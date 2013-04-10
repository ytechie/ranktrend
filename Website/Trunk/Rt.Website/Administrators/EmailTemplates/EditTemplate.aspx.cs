using System;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;

public partial class Administrators_EmailTemplates_EditTemplate : Page
{
	private const string PARAM_ID = "Id";

	private int? EmailTemplateIdParameter
	{
		get
		{
			int id;
			object idParam = Request.QueryString[PARAM_ID];

			if (idParam == null) return null;

			if (int.TryParse(idParam.ToString(), out id))
				return id;
			else
				return null;
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initEmailTemplate();
			initTokens();
		}
	}

	protected void Save_Click(object sender, EventArgs e)
	{
		if (Page.IsValid)
		{
			save(false);
			redirectHome();
		}
	}

	protected void SaveAndPreview_Click(object sender, EventArgs e)
	{
		if (Page.IsValid)
		{
			save(true);
			redirectHome();
		}
	}

	protected void Cancel_Click(object sender, EventArgs e)
	{
		redirectHome();
	}

	private void save(bool preview)
	{
		Database db = Global.GetDbConnection();
		int? id = EmailTemplateIdParameter;
		var emailTemplate = new EmailTemplate();

		emailTemplate.Subject = Subject.Text;
		emailTemplate.Html = Html.Checked;
		emailTemplate.Message = Message.Text;
		emailTemplate.Locked = Lock.Checked;

		if (id != null)
			emailTemplate.Id = id;

		db.SaveEmailTemplate(emailTemplate);

		if (preview)
			sendPreview(emailTemplate);
	}

	private static void sendPreview(EmailTemplate emailTemplate)
	{
		Database db = Global.GetDbConnection();
		var emailMessage = new EmailMessage(emailTemplate);
		MembershipUser currUser = Membership.GetUser();
		var userInformation = db.ORManager.Get<UserInformation>(currUser.ProviderUserKey);

		emailMessage.ApplyToUser(currUser, userInformation);
		emailMessage.From = GlobalSettings.AdministrativeEmail;

		db.SaveEmail(emailMessage);
		ThreadPool.QueueUserWorkItem(forceEmailEngineRun);
	}

	private static void forceEmailEngineRun(object state)
	{
		RtEngines.EmailEngine.RunFullCycle();
	}

	private void redirectHome()
	{
		Response.Redirect(Administrators_EmailTemplates_Default.GetLoadUrl());
	}

	private void initEmailTemplate()
	{
		int? id = EmailTemplateIdParameter;

		if (id != null)
		{
			Database db = Global.GetDbConnection();
			EmailTemplate emailTemplate = db.GetEmailTemplate((int) id);

			EmailTemplateId.Text = emailTemplate.Id.ToString();
			Subject.Text = emailTemplate.Subject;
			Html.Checked = emailTemplate.Html;
			Message.Text = emailTemplate.Message;
			Lock.Visible = !emailTemplate.Locked;
			Lock.Checked = emailTemplate.Locked;
			Locked.Visible = emailTemplate.Locked;
		}
		else
		{
			Lock.Visible = true;
			Lock.Checked = false;
			Locked.Visible = false;
		}
	}

	private void initTokens()
	{
		Database db = Global.GetDbConnection();
		MembershipUser user = Membership.GetUser();
		var uinfo = db.ORManager.Get<UserInformation>(user.ProviderUserKey);

		HomepageUrl.Text = EmailMessage.TOKEN_HOMEPAGE_URL;
		HomepageUrlExample.Text = EmailMessage.ReplaceGeneralTokens(EmailMessage.TOKEN_HOMEPAGE_URL);

		HomepageLink.Text = EmailMessage.TOKEN_HOMEPAGE_LINK;
		HomepageLinkExample.Text = EmailMessage.ReplaceGeneralTokens(EmailMessage.TOKEN_HOMEPAGE_LINK);

		Logo.Text = EmailMessage.TOKEN_LOGO;
		LogoExample.Text = EmailMessage.ReplaceGeneralTokens(EmailMessage.TOKEN_LOGO);

		UsersUid.Text = EmailMessage.TOKEN_USERUID;
		UsersUidExample.Text = EmailMessage.ApplyToUser(EmailMessage.TOKEN_USERUID, user, uinfo);

		UsersFirstName.Text = EmailMessage.TOKEN_FIRSTNAME;
		UsersFirstNameExample.Text = EmailMessage.ApplyToUser(EmailMessage.TOKEN_FIRSTNAME, user, uinfo);

		UsersLastName.Text = EmailMessage.TOKEN_LASTNAME;
		UsersLastNameExample.Text = EmailMessage.ApplyToUser(EmailMessage.TOKEN_LASTNAME, user, uinfo);

		UsersFullName.Text = EmailMessage.TOKEN_FULLNAME;
		UsersFullNameExample.Text = EmailMessage.ApplyToUser(EmailMessage.TOKEN_FULLNAME, user, uinfo);

		UsersUsername.Text = EmailMessage.TOKEN_USERNAME;
		UsersUsernameExample.Text = EmailMessage.ApplyToUser(EmailMessage.TOKEN_USERNAME, user, uinfo);

		UsersEmail.Text = EmailMessage.TOKEN_EMAIL;
		UsersEmailExample.Text = EmailMessage.ApplyToUser(EmailMessage.TOKEN_EMAIL, user, uinfo);
	}
}