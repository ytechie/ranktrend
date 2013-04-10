using System;
using System.Data;
using System.Reflection;
using System.Web.UI;
using log4net;
using Rt.Framework.CommonControls.Web;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Web;
using Rt.Website;
using YTech.General.Collections;
using YTech.General.Web;

public partial class Members_Reports_Thumbnail_Dashboard_Default : Page
{
	/// <summary>
	///		Declare and create our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected void Page_Load(object sender, EventArgs e)
	{
		cmdDrillDownMulti.Click += cmdDrillDownMulti_Click;

		if (!Page.IsPostBack)
			siteList.PopulatePageList();

		if (siteList.GetSelectedSiteId() != null)
			addThumbnails();
	}

	private void cmdDrillDownMulti_Click(object sender, EventArgs e)
	{
		DelimitedList dsList;
		string datasourceList;

		dsList = new DelimitedList();

		//Find all of the thumbnail section controls
		foreach (Control currControl in thumbnails.Controls)
		{
			Members_Reports_Thumbnail_Dashboard_Thumbnail_Section thumbControl;

			thumbControl = (Members_Reports_Thumbnail_Dashboard_Thumbnail_Section) currControl;

			//Build a list of the datasource ids
			if (thumbControl.Selected)
				dsList.Append(thumbControl.DatasourceIdString);
		}

		datasourceList = dsList.ToString();
		if (datasourceList.Length == 0)
			return;

		_log.DebugFormat("Built a data source list of '{0}'", dsList.ToString());

		drillDownMultiple(datasourceList);
	}

	private void drillDownMultiple(string datasourceList)
	{
		UrlBuilder url;
		DateTime start, end;
		DateRangeSelector.DateRanges dateRange;

		url = new UrlBuilder();
		url.URLBase = ResolveUrl("~/Members/Interactive-Report/");
		url.Parameters.AddParameter(QueryParameters.QUERY_DATASOURCE_LIST, datasourceList);

		dateSelector.GetTimeRange(out start, out end, out dateRange);

		if (dateRange == DateRangeSelector.DateRanges.Custom)
		{
			url.Parameters.AddParameter(QueryParameters.QUERY_START, start);
			url.Parameters.AddParameter(QueryParameters.QUERY_END, end);
		}
		else
		{
			url.Parameters.AddParameter(QueryParameters.QUERY_TIME_RANGE, ((int) dateRange).ToString());
		}

		Response.Redirect(url.ToString());
	}

	private void addThumbnails()
	{
		DataTable datasourceList;
		Database db;

		db = Global.GetDbConnection();

		datasourceList = db.Ds_GetPageDatasourceList(siteList.GetSelectedSiteId().Value, true);

		_log.Debug("About to generate thumbnails");

		foreach (DataRow currRow in datasourceList.Rows)
		{
			Members_Reports_Thumbnail_Dashboard_Thumbnail_Section currThumb;
			int datasourceId;
			int? subTypeId;
			string datasourceName;

			datasourceId = (int) currRow["Id"];
			if (currRow["SubTypeId"] == DBNull.Value)
				subTypeId = null;
			else
				subTypeId = (int) currRow["SubTypeId"];
			datasourceName = (string) currRow["DisplayName"];

			currThumb = LoadControl("Thumbnail-Section.ascx") as Members_Reports_Thumbnail_Dashboard_Thumbnail_Section;

			if (currThumb != null)
			{
				currThumb.Render(datasourceId, subTypeId, datasourceName, dateSelector);
				thumbnails.Controls.Add(currThumb);
			}
		}

		_log.Debug("Thumbnail generation complete");
	}
}