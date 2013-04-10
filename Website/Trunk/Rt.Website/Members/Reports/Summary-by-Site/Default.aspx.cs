using System;
using System.Web.UI;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Reports_Summary_by_Site_Default : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		siteList.SelectedItemChanged += siteList_SelectedItemChanged;

		if (!Page.IsPostBack)
		{
			siteList.PopulatePageList();
			populateReport();
		}
	}

	private void siteList_SelectedItemChanged(object sender, EventArgs e)
	{
		populateReport();
	}

	private void populateReport()
	{
		int? siteId;

		siteId = siteList.GetSelectedSiteId();
		if (siteId == null)
			return;

		summaryTable.setDatasource(_db, siteId.Value);
	}
}