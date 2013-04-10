using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Administrators_EmailTemplates_SendEmail : Page
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
			if (EmailTemplateIdParameter != null)
			{
				initTo();
				initEmail();
			}
			else
				redirectHome();
		}
	}

	protected void Send_Click(object sender, EventArgs e)
	{
		Database db = Global.GetDbConnection();
		db.SendMassEmail((int) EmailTemplateIdParameter, GlobalSettings.AdministrativeEmail, To.Text,
		                 Membership.ApplicationName);
		redirectHome();
	}

	protected void Cancel_Click(object sender, EventArgs e)
	{
		redirectHome();
	}

	private void initTo()
	{
		foreach (string role in Roles.GetAllRoles())
		{
			var item = new ListItem(role, role);
			To.Items.Add(item);
		}

		ICriteria criteria;
		IList<EmailFilter> filters;

		Database db = Global.GetDbConnection();
		criteria = db.ORManager.Session.CreateCriteria(typeof (EmailFilter));
		filters = criteria.List<EmailFilter>();

		foreach (EmailFilter filter in filters)
		{
			var newItem = new ListItem(filter.DisplayName, filter.Name);
			To.Items.Add(newItem);
		}
	}

	private void initEmail()
	{
		Database db = Global.GetDbConnection();
		EmailTemplate template = db.GetEmailTemplate((int) EmailTemplateIdParameter);
		var email = new EmailMessage(template);

		EmailTemplateId.Text = template.Id.ToString();
		From.Text = GlobalSettings.AdministrativeEmail;
		Subject.Text = template.Subject;
		MessageFormat.Text = template.Html ? "HTML" : "Plain-Text";
		MessageBody.Text = template.Html ? email.Message : Server.HtmlEncode(email.Message);
	}

	private void redirectHome()
	{
		Response.Redirect(Administrators_EmailTemplates_Default.GetLoadUrl());
	}
}