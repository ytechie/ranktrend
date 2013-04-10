using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChartDirector;
using log4net;
using MagicAjax;
using Rt.Framework.Applications.InteractiveReport;
using Rt.Framework.CommonControls.Web;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Web;
using Rt.Website;

public partial class Members_Interactive_Report_Default : Page
{
	/// <summary>
	///		This string value is used for listboxes that need an "any" value.  This value
	///		could be changed to any string in theory.
	/// </summary>
	private const string ANY_VALUE = "any";

	private const string VIEWSTATE_EDITING_DATASOURCE_ID = "Editing_DS_Id";
	private const string VIEWSTATE_EXTRA_DATASOURCE_OPTIONS = "Extra_DS_Options_{0}";

	/// <summary>
	///		Declare and create our logger for this class.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Database _db;
	private bool _rendered;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		Global.AddCommonJavaScript(this);
		Global.AddTooltipJavaScript(this);
		ClientScript.RegisterClientScriptInclude("interactiveReport", "Script.js");

		_db = Global.GetDbConnection();
		_userId = (Guid) Membership.GetUser().ProviderUserKey;

		setUpTabs();

		//dgDatasources.ItemCommand += new DataGridCommandEventHandler(dgDatasources_ItemCommand);
		chartViewer.ClickHotSpot += chartViewer_ClickHotSpot;
		PreRender += Members_Interactive_Report_Default_PreRender;

		savedReportTab.SettingsLoaded += savedReportTab_SettingsLoaded;
		savedReportTab.ChartRefreshRequest += savedReportTab_ChartRefreshRequest;

		if (!Page.IsPostBack)
			loadQueryParameters();
	}

	private void loadQueryParameters()
	{
		Dictionary<string, object> parameters;

		parameters = QueryParameters.ReadCommonQueryParameters(Request.QueryString);

		if (parameters.ContainsKey(QueryParameters.QUERY_TIME_RANGE))
		{
			DateRangeSelector.DateRanges dateRange;

			dateRange = (DateRangeSelector.DateRanges) parameters[QueryParameters.QUERY_TIME_RANGE];
			overviewTab.DateSelector.SetTimeRange(DateTime.Now.AddMonths(-1), DateTime.Now, dateRange);
		}
		else if (parameters.ContainsKey(QueryParameters.QUERY_START) && parameters.ContainsKey(QueryParameters.QUERY_END))
		{
			DateTime start, end;

			start = (DateTime) parameters[QueryParameters.QUERY_START];
			end = (DateTime) parameters[QueryParameters.QUERY_END];

			overviewTab.DateSelector.SetTimeRange(start, end);
		}

		if (parameters.ContainsKey(QueryParameters.QUERY_DATASOURCE_LIST))
		{
			QueryParameters.DatasourceIds[] ids;
			DisplayDatasourceItem[] datasources;

			ids = (QueryParameters.DatasourceIds[]) parameters[QueryParameters.QUERY_DATASOURCE_LIST];
			datasources = new DisplayDatasourceItem[ids.Length];

			for (int i = 0; i < ids.Length; i++)
			{
				datasources[i] = new DisplayDatasourceItem();
				datasources[i].ConfiguredDatasourceId = ids[i].DatasourceId;
				datasources[i].DatasourceSubTypeId = ids[i].SubTypeId;
				datasources[i].ShowRaw = true;
			}

			datasourceList.PopulateDatasources();
			datasourceList.SetSelectedDatasources(datasources);
		}
	}

	private void Members_Interactive_Report_Default_PreRender(object sender, EventArgs e)
	{
		checkChartRefresh();
	}

	private void savedReportTab_ChartRefreshRequest(object sender, EventArgs e)
	{
		refreshChart();
	}

	private void chartViewer_ClickHotSpot(object sender, WebHotSpotEventArgs e)
	{
		string buttonKey;

		buttonKey = (string) e.AttrValues["cookie"];

		if (buttonKey == RankChartGenerator.TOOLBAR_KEY_REFRESH)
		{
			refreshChart();
		}
		else if (buttonKey == RankChartGenerator.TOOLBAR_KEY_ZOOM_IN)
		{
			overviewTab.DateSelector.ZoomIn();
			refreshChart();
		}
		else if (buttonKey == RankChartGenerator.TOOLBAR_KEY_ZOOM_OUT)
		{
			overviewTab.DateSelector.ZoomOut();
			refreshChart();
		}
		else if (buttonKey == RankChartGenerator.TOOLBAR_KEY_SCROLL_LEFT)
		{
			overviewTab.DateSelector.ScrollLeft();
			refreshChart();
		}
		else if (buttonKey == RankChartGenerator.TOOLBAR_KEY_SCROLL_RIGHT)
		{
			overviewTab.DateSelector.ScrollRight();
			refreshChart();
		}
		else if (buttonKey == RankChartGenerator.TOOLBAR_KEY_SAVE)
		{
			saveDisplayedParameters();
			refreshChart();
		}
	}

	//Checks if the chart was rendered previously, and needs to be re-rendered
	private void checkChartRefresh()
	{
		if (MagicAjaxContext.Current.IsAjaxCall)
			return;

		refreshChart();
	}

	private void refreshChart()
	{
		RankChartGenerator generator;
		RankChartParameters rcp;
		string reportName;

		if (_rendered)
			return;

		getCurrentParameters(out rcp, out reportName);

		if (string.IsNullOrEmpty(reportName))
			generator = new RankChartGenerator(Global.GetDbConnection(), rcp);
		else
			generator = new RankChartGenerator(Global.GetDbConnection(), rcp, overviewTab.ReportName);

		if (rcp.ChartSize == RankChartParameters.ChartSizes.Small)
			generator.NoDatasourceMessage = "You have not chosen any datasources.\nPlease choose some datasources to\n" +
			                                "display from the list below, and press\nthe refresh button to the left of this chart";
		else
			generator.NoDatasourceMessage = "You have not chosen any datasources.  Please choose some datasources to\n" +
			                                "display from the list below, and press the refresh button to the left of this chart";
		generator.ToolbarImagePath = Server.MapPath("Toolbar-Icons");

		generator.GenerateChart(chartViewer);

		_rendered = true;
	}

	#region Tab Infrastructure

	private void setUpTabs()
	{
		cmdOverview.ServerClick += cmdOverview_ServerClick;
		cmdData.ServerClick += cmdData_ServerClick;
		cmdEvents.ServerClick += cmdEvents_ServerClick;
		cmdSaved.ServerClick += cmdSaved_ServerClick;

		if (!Page.IsPostBack)
			showPanel(pnlSaved);
	}

	private void showPanel(Panel p)
	{
		pnlOverview.Visible = (p == pnlOverview);
		setTabCss((HtmlTableCell) cmdOverview.Parent, (p == pnlOverview));

		pnlData.Visible = (p == pnlData);
		setTabCss((HtmlTableCell) cmdData.Parent, (p == pnlData));

		pnlEvents.Visible = (p == pnlEvents);
		setTabCss((HtmlTableCell) cmdEvents.Parent, (p == pnlEvents));

		pnlSaved.Visible = (p == pnlSaved);
		setTabCss((HtmlTableCell) cmdSaved.Parent, (p == pnlSaved));
	}

	private void setTabCss(HtmlTableCell cell, bool selected)
	{
		if (selected)
			cell.Attributes["class"] = "TabTable_Selected";
		else
			cell.Attributes["class"] = "";
	}

	private void cmdSaved_ServerClick(object sender, EventArgs e)
	{
		showPanel(pnlSaved);

		savedReportTab.PopulateReportList();
	}

	private void cmdEvents_ServerClick(object sender, EventArgs e)
	{
		showPanel(pnlEvents);
	}

	private void cmdData_ServerClick(object sender, EventArgs e)
	{
		showPanel(pnlData);
	}

	private void cmdOverview_ServerClick(object sender, EventArgs e)
	{
		showPanel(pnlOverview);
	}

	#endregion

	#region Chart Parameters / Settings

	private void savedReportTab_SettingsLoaded(object sender, EventArgs e)
	{
		setCurrentParameters(savedReportTab.LoadedParameters, savedReportTab.GetDisplayedSettings());
	}

	private void saveDisplayedParameters()
	{
		RankChartParameters parameters;
		string reportName;

		getCurrentParameters(out parameters, out reportName);

		savedReportTab.SaveSettings(parameters, reportName);

		//Update the last saved time
		overviewTab.ReportLastSaved = DateTime.UtcNow;
	}

	private void getCurrentParameters(out RankChartParameters parameters, out string reportName)
	{
		DateTime start, end;
		DateRangeSelector.DateRanges dateRangeType;

		parameters = new RankChartParameters();

		//Overview Tab
		overviewTab.DateSelector.GetTimeRange(out start, out end, out dateRangeType);
		parameters.DateRangeType = dateRangeType;
		parameters.StartTime = start;
		parameters.EndTime = end;
		parameters.ChartSize = overviewTab.ChartSize;

		//Datasources tab
		parameters.Datasources = datasourceList.GetSelectedDatasources();

		//Events tab
		parameters.EventCategories = eventCategoryList.GetSelectedEventCategories();

		reportName = overviewTab.ReportName;
	}

	private void setCurrentParameters(RankChartParameters rcp, SavedReport savedReport)
	{
		overviewTab.DisplaySettings(savedReport);

		if (rcp != null)
		{
			//A couple of old reports might not have a range value, so we'll just use custom
			//for backwards compatibility
			if (rcp.DateRangeType == 0)
				rcp.DateRangeType = DateRangeSelector.DateRanges.Custom;

			overviewTab.DateSelector.SetTimeRange(rcp.StartTime, rcp.EndTime, rcp.DateRangeType);
			overviewTab.ChartSize = rcp.ChartSize;
			datasourceList.SetSelectedDatasources(rcp.Datasources);
			eventCategoryList.SetSelectedEventCategories(rcp.EventCategories);
		}
	}

	#endregion
}