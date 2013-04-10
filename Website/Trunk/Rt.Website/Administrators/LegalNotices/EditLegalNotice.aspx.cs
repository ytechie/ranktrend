using System;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web.JavaScript;

public partial class Administrators_LegalNotices_EditLegalNotice : Page
{
	private const string PARAM_ID = "Id";

	private int? LegalNoticeIdParameter
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
			initJavascript();
			initLegalNotice();
		}
	}

	protected void Save_Click(object sender, EventArgs e)
	{
		if (Page.IsValid)
		{
			save();
			redirectHome();
		}
	}

	protected void Cancel_Click(object sender, EventArgs e)
	{
		redirectHome();
	}

	private void initJavascript()
	{
		JavaScriptBlock.ConfirmClick(Save,
		                             "Performing this action will effectively cause all users to be reset on their acceptance of this legal notice.\\nAre you sure you want to perform this action?");
	}

	private void initLegalNotice()
	{
		int? id = LegalNoticeIdParameter;

		if (id != null)
		{
			Database db = Global.GetDbConnection();
			LegalNotice notice = db.GetLegalNotice((int) id);
			LegalNoticeVersion version = db.GetLatestLegalNoticeVersion((int) notice.Id);

			LegalNoticeId.Text = notice.Id.ToString();
			Description.Text = notice.Description;
			Notice.Text = version == null ? string.Empty : version.Notice;
		}
		else
		{
			Response.Redirect(Administrators_LegalNotices_Default.GetLoadUrl());
		}
	}

	private void redirectHome()
	{
		Response.Redirect(Administrators_LegalNotices_Default.GetLoadUrl());
	}

	private void save()
	{
		Database db = Global.GetDbConnection();
		int? id = LegalNoticeIdParameter;
		var legalNoticeVersion = new LegalNoticeVersion();

		legalNoticeVersion.LegalNoticeId = (int) id;
		legalNoticeVersion.Notice = Notice.Text;

		db.SaveLegalNoticeVersion(legalNoticeVersion);
	}
}