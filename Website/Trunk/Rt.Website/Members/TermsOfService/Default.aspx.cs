using System;
using System.Web.Security;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_TermsOfService_Default : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initTos();
		}
	}

	protected void Save_Click(object sender, EventArgs e)
	{
		if (Page.IsValid)
		{
			saveTosAgreement();
			redirectHome();
		}
	}

	private void initTos()
	{
		Database db = Global.GetDbConnection();
		LegalNoticeVersion termsOfService = db.GetLatestLegalNoticeVersion(GlobalSettings.SignUpTermsOfService);
		TermsOfService.Text = termsOfService.Notice;
	}

	private void saveTosAgreement()
	{
		Database db = Global.GetDbConnection();
		MembershipUser membershipUser = Membership.GetUser();

		// Save user's agreement to the TOS
		LegalNoticeVersion termsOfService = db.GetLatestLegalNoticeVersion(GlobalSettings.SignUpTermsOfService);
		var tosAgreement = new LegalNoticeAgreement(termsOfService, membershipUser, true);
		db.SaveLegalNoticeAgreement(tosAgreement);
	}

	private void redirectHome()
	{
		Response.Redirect("~/Members/");
	}
}