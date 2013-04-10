using System;
using System.Data;
using System.Reflection;
using System.Web.UI;
using log4net;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Reports_Raw_Data_View_Default : Page
{
	/// <summary>
	///		Declare and create our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		siteList.SelectedItemChanged += siteList_SelectedItemChanged;
		cmdGo.Click += cmdGo_Click;

		if (!Page.IsPostBack)
		{
			siteList.PopulatePageList();
			configuredDatasourceList.PopulateDatasourceList(siteList.GetSelectedSite());
			configuredDatasourceList.ShowSubTypeList();
		}
	}

	private void cmdGo_Click(object sender, EventArgs e)
	{
		dgRawData.DataSource = queryData();
		dgRawData.DataBind();
	}

	private DataTable queryData()
	{
		DateTime start, end;
		int? configuredDatasourceId;
		int? subTypeId;
		DataTable dt;
		TimeSpan utcOffset;

		configuredDatasourceId = configuredDatasourceList.GetSelectedDatasourceId();
		subTypeId = configuredDatasourceList.GetSelectedSubTypeId();
		dateSelector.GetTimeRange(out start, out end);

		if (configuredDatasourceId == null)
		{
			dt = new DataTable();
		}
		else
		{
			_log.DebugFormat("Retrieving data for configured datasource id #{0}, sub type #{1}, start: {1}, end: {2}",
			                 configuredDatasourceId, subTypeId, start, end);
			dt = _db.Report_RawDataView(configuredDatasourceId, subTypeId, start, end.AddDays(1));
			_log.Debug("Data retreival complete");

			//Get the UTC offset for the user
			utcOffset = Global.GetUserUtcOffset();

			foreach (DataRow currRow in dt.Rows)
				currRow["Timestamp"] = ((DateTime) currRow["Timestamp"]).Add(utcOffset);

			_log.Debug("Time zones successfully converted to the users local time");
		}

		if (ddlSortDirection.SelectedValue == "Desc")
		{
			DataView sort;

			_log.Debug("The data should be in descending order, begining sort");

			sort = new DataView(dt);
			sort.Sort = "Timestamp Desc";
			dt = sort.ToTable();

			_log.Debug("Data sort complete");
		}

		return dt;
	}

	private void siteList_SelectedItemChanged(object sender, EventArgs e)
	{
		configuredDatasourceList.PopulateDatasourceList(siteList.GetSelectedSite());
	}
}