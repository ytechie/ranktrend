using System;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class TermsOfService_Default : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initTos();
		}
	}

	private void initTos()
	{
		Database db = Global.GetDbConnection();
		LegalNoticeVersion tos = db.GetLatestLegalNoticeVersion(GlobalSettings.SignUpTermsOfService);
		Tos.Text = tos.Notice.Replace("\n", "<br />").Replace("   ", "&nbsp;&nbsp;&nbsp;");
	}
}