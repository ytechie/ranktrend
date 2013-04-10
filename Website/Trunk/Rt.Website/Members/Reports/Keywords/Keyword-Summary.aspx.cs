using System;
using System.Data;
using System.Web.UI;
using Rt.Framework.Applications.Keywords;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Reports_Keywords_Keyword_Summary : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		int? siteId;

		_db = Global.GetDbConnection();

		if (!Page.IsPostBack)
			siteList.PopulatePageList();

		siteId = siteList.GetSelectedSiteId();
		if (siteId != null)
		{
			DataTable[] tables;
			DataTable displayData;

			tables = loadData(siteId.Value);
			displayData = processData(tables);
			populateTable(displayData);
		}
	}

	private DataTable[] loadData(int siteId)
	{
		DataTable[] tables;

		tables = new DataTable[3];
		tables[0] = _db.Keywords_GetKeywordList(siteId, 1);
		tables[0].TableName = "Yahoo";
		tables[1] = _db.Keywords_GetKeywordList(siteId, 2);
		tables[1].TableName = "Google";
		tables[2] = _db.Keywords_GetKeywordList(siteId, 7);
		tables[2].TableName = "MS Live";

		return tables;
	}

	private DataTable processData(DataTable[] keywordLists)
	{
		DataTable processedData;

		processedData = KeywordSummary.CombineKeywordLists(keywordLists);

		return processedData;
	}

	private void populateTable(DataTable displayData)
	{
		if (displayData.Rows.Count > 0)
		{
			noDataPanel.Visible = false;
			dgKeywordTable.Visible = true;

			dgKeywordTable.DataSource = displayData;
			dgKeywordTable.DataBind();
		}
		else
		{
			noDataPanel.Visible = true;
			dgKeywordTable.Visible = false;
		}
	}
}