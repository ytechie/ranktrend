using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Keywords_Keyword_Bulk_Import : Page
{
	private int? _selectedSiteId;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
			siteList.PopulatePageList();

		_selectedSiteId = siteList.GetSelectedSiteId();
		if (_selectedSiteId == null)
		{
			pnlStandard.Visible = false;
			pnlNoSites.Visible = true;
		}

		cmdImport.Click += cmdImport_Click;
	}

	private void cmdImport_Click(object sender, EventArgs e)
	{
		string[] keywords;
		List<int> datasourceTypeIds;

		if (txtKeywords.Text.Length == 0)
			return;

		keywords = txtKeywords.Text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

		//Get the list of datasource types
		datasourceTypeIds = new List<int>();
		foreach (ListItem currItem in searchEngineList.Items)
			if (currItem.Selected)
				datasourceTypeIds.Add(int.Parse(currItem.Value));

		addKeywords(_selectedSiteId.Value, keywords, datasourceTypeIds.ToArray());

		//Redirect to the summary report
		Response.Redirect("../Reports/Keywords/Keyword-Summary.aspx");
	}

	private static void addKeywords(int urlId, string[] keywords, int[] datasourceTypeIds)
	{
		Database db;

		db = Global.GetDbConnection();
		foreach (int datasourceTypeId in datasourceTypeIds)
			db.Keywords_BulkImport(urlId, keywords, datasourceTypeId);
	}
}