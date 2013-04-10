using System;
using System.Web.UI;
using Rt.Framework.Applications.Thumbnails;
using Rt.Framework.CommonControls.Web;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Web;
using Rt.Website;
using YTech.General.Web;

public partial class Members_Reports_Thumbnail_Dashboard_Thumbnail_Section : UserControl
{
	private const string ATTRIBUTE_DATASOURCE_ID_STRING = "dsid";

	public string DatasourceIdString
	{
		get { return chkTitle.Attributes[ATTRIBUTE_DATASOURCE_ID_STRING]; }
		set { chkTitle.Attributes[ATTRIBUTE_DATASOURCE_ID_STRING] = value; }
	}

	public bool Selected
	{
		get { return chkTitle.Checked; }
		set { chkTitle.Checked = value; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	public void Render(int configuredDatasourceId, int? subTypeId, string title, DateRangeSelector dateSelector)
	{
		ThumbnailGenerator gen;
		Database db;
		string url;
		string datasourceList;
		DateTime start, end;
		DateRangeSelector.DateRanges dateRange;
		UrlBuilder clickUrl;

		db = Global.GetDbConnection();

		dateSelector.GetTimeRange(out start, out end, out dateRange);

		gen = new ThumbnailGenerator(db);

		//bytes = gen.Generate(configuredDatasourceId, subTypeId, start, end);
		//url = Common_SessionImage.StoreImageForDisplay(bytes, ViewState);

		url = gen.Generate(configuredDatasourceId, subTypeId, start, end);

		imgThumb.Src = url;
		imgThumb.Alt = title;
		chkTitle.Text = title;

		datasourceList = configuredDatasourceId.ToString();
		if (subTypeId != null)
			datasourceList += "." + subTypeId;

		clickUrl = new UrlBuilder();
		clickUrl.URLBase = ResolveUrl("~/Members/Interactive-Report/");
		clickUrl.Parameters.AddParameter(QueryParameters.QUERY_DATASOURCE_LIST, datasourceList);

		//Store the ID information in an attribute
		DatasourceIdString = datasourceList;

		if (dateRange == DateRangeSelector.DateRanges.Custom)
		{
			clickUrl.Parameters.AddParameter(QueryParameters.QUERY_START, start);
			clickUrl.Parameters.AddParameter(QueryParameters.QUERY_END, end);
		}
		else
		{
			clickUrl.Parameters.AddParameter(QueryParameters.QUERY_TIME_RANGE, ((int) dateRange).ToString());
		}

		lnkDrillDown.HRef = clickUrl.ToString();
	}
}