using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Applications.InteractiveReport;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web.Controls;
using YTech.General.Web.JavaScript;

public partial class Members_Interactive_Report_SavedReportTab : UserControl, IPostBackEventHandler
{
	private const string VIEWSTATE_DISPLAYED_SETTINGS_ID = "saved_dsid";
	private const string VIEWSTATE_REPORT_LIST_POPULATED = "rlpop";
	private Database _db;
	private RankChartParameters _rcp;
	private Guid _userId;

	public RankChartParameters LoadedParameters
	{
		get { return _rcp; }
		set { _rcp = value; }
	}

	#region IPostBackEventHandler Members

	public void RaisePostBackEvent(string eventArgument)
	{
		if (eventArgument == "lstReports_doubleClick")
			loadSelected();
	}

	#endregion

	public event EventHandler SettingsLoaded;
	public event EventHandler ChartRefreshRequest;

	protected void Page_Load(object sender, EventArgs e)
	{
		_userId = (Guid) Membership.GetUser().ProviderUserKey;
		getDbConnection();

		cmdLoad.Click += cmdLoad_Click;
		cmdDelete.Click += cmdDelete_Click;

		if (!Page.IsPostBack)
		{
			PopulateReportList();

			//Confirm the delete click
			JavaScriptBlock.ConfirmClick(cmdDelete,
			                             "Are you sure you want to delete this saved report? It will also be removed from any custom reports that it belongs to.");

			//Enable double-click loading
			lstReports.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(this, "lstReports_doubleClick");

			//Load the default settings
			raiseSettingsLoaded();
		}
	}

	private void getDbConnection()
	{
		_db = Global.GetDbConnection();
	}

	private void cmdDelete_Click(object sender, EventArgs e)
	{
		int settingsId;
		int deletedIndex;

		if (lstReports.SelectedValue == string.Empty)
			return;

		if (lstReports.Items.Count == 1)
			return;

		settingsId = int.Parse(lstReports.SelectedValue);

		_db.SavedReports_Delete(settingsId);

		//Remove the report from the list
		deletedIndex = lstReports.SelectedIndex;
		lstReports.Items.Remove(lstReports.SelectedItem);

		//Select the next item
		lstReports.SelectedIndex =
			ListUtilities.GetItemIndexAfterDelete(lstReports.Items.Count, deletedIndex);

		loadSelected();
	}

	private void raiseSettingsLoaded()
	{
		EventHandler eh;

		eh = SettingsLoaded;
		if (eh != null)
			eh(this, new EventArgs());
	}

	private void raiseChartRefreshRequest(object s, EventArgs e)
	{
		EventHandler eh;

		eh = ChartRefreshRequest;
		if (eh != null)
			eh(s, e);
	}

	private void cmdLoad_Click(object sender, EventArgs e)
	{
		loadSelected();
	}

	private void loadSelected()
	{
		SavedReport report;

		report = getSelectedReportSettings();

		//Remember which settings are saved
		if (report.Id != null)
		{
			setDisplayedSettingsId(report.Id.Value);

			_rcp = RankChartParameters.XmlDeserialize(report.XmlData);
		}
		else
		{
			setDisplayedSettingsId(null);

			_rcp = new RankChartParameters();
			_rcp.Datasources = new DisplayDatasourceItem[0];
			_rcp.EventCategories = new DisplayEventCategoryItem[0];
		}

		raiseSettingsLoaded();
		raiseChartRefreshRequest(this, null);
	}

	private int? getDisplayedSettingsId()
	{
		return ViewState[VIEWSTATE_DISPLAYED_SETTINGS_ID] as int?;
	}

	private void setDisplayedSettingsId(int? settingsId)
	{
		ViewState[VIEWSTATE_DISPLAYED_SETTINGS_ID] = settingsId;
	}

	public SavedReport GetDisplayedSettings()
	{
		int? settingsId;

		settingsId = getDisplayedSettingsId();
		if (settingsId == null)
			return getNewSavedReport();

		if (_db == null)
			getDbConnection();

		return _db.ORManager.Get<SavedReport>(settingsId.Value);
	}

	private SavedReport getSelectedReportSettings()
	{
		int selectedId;

		if (lstReports.SelectedIndex == -1)
			return null;

		if (lstReports.SelectedValue == string.Empty)
		{
			//Create a new saved report
			return getNewSavedReport();
		}

		selectedId = int.Parse(lstReports.SelectedValue);

		return _db.ORManager.Get<SavedReport>(selectedId);
	}

	private static SavedReport getNewSavedReport()
	{
		SavedReport settings;

		settings = new SavedReport();
		settings.Created = DateTime.UtcNow;
		settings.Name = string.Format("Report Created {0:d}", DateTime.UtcNow);
		settings.ReportTypeId = 1;

		return settings;
	}

	public void SaveSettings(RankChartParameters settings, string reportName)
	{
		SavedReport sr;
		int? settingsId;
		bool newSettings;

		settingsId = getDisplayedSettingsId();

		newSettings = settingsId == null;

		if (newSettings)
		{
			sr = new SavedReport();
			sr.Created = DateTime.UtcNow;
		}
		else
		{
			//Load the options to set the fields we're not explicitly setting
			sr = _db.ORManager.Get<SavedReport>((int) settingsId);
		}

		sr.XmlData = settings.XmlSerialize();
		sr.UserId = _userId;
		sr.ReportTypeId = 1;
		sr.LastSaved = DateTime.UtcNow;

		if (sr.Name != reportName)
		{
			sr.Name = reportName;

			if (!newSettings)
				lstReports.SelectedItem.Text = reportName;
		}

		_db.ORManager.SaveOrUpdate(sr);

		if (newSettings)
		{
			//Add the item to the report list
			lstReports.Items.Add(new ListItem(sr.Name, sr.Id.ToString()));
			lstReports.SelectedIndex = lstReports.Items.Count - 1;

			//Set the saved report as the one that's displayed, so that
			//subsequent saves overwrite the settings.
			setDisplayedSettingsId(sr.Id.Value);
		}
	}

	public void PopulateReportList()
	{
		ICriteria criteria;
		IList<SavedReport> savedReports;

		//Don't populate the list twice
		if (ViewState[VIEWSTATE_REPORT_LIST_POPULATED] != null)
			return;

		criteria =
			_db.ORManager.Session.CreateCriteria(typeof (SavedReport)).Add(Expression.Eq("UserId", _userId)).AddOrder(
				Order.Asc("Name"));
		savedReports = criteria.List<SavedReport>();

		lstReports.DataSource = savedReports;
		lstReports.DataTextField = "Name";
		lstReports.DataValueField = "Id";
		lstReports.DataBind();

		if (lstReports.Items.Count == 0)
			cmdLoad.Enabled = false;
		else
			cmdLoad.Enabled = true;

		//Add the new... item
		lstReports.Items.Insert(0, new ListItem("New...", string.Empty));
		lstReports.SelectedIndex = 0;

		ViewState[VIEWSTATE_REPORT_LIST_POPULATED] = true;
	}
}