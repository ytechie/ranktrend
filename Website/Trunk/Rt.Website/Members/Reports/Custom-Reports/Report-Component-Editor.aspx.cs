using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web.Controls;
using YTech.General.Web.JavaScript;

public partial class Members_Reports_Custom_Reports_Report_Component_Editor : Page
{
	private const string PARAMETER_CUSTOM_REPORT_ID = "Custom-Report-Id";

	/// <summary>
	///		Create and declare our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private int _customReportId;
	private Database _db;
	private CustomReport _report;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		parseParameters();

		_userId = (Guid) Membership.GetUser().ProviderUserKey;
		_db = Global.GetDbConnection();

		//Load the report
		_report = _db.ORManager.Get<CustomReport>(_customReportId);

		//Set the URL for the preview link
		initPreviewLink();

		lstReportParts.SelectedIndexChanged += lstReportParts_SelectedIndexChanged;
		ddlReportType.SelectedIndexChanged += ddlReportType_SelectedIndexChanged;

		cmdAdd.Click += cmdAdd_Click;
		cmdDelete.Click += cmdDelete_Click;
		cmdSave.Click += cmdSave_Click;

		//Verify that the report belongs to this user
		if (_report.UserId != _userId)
			throw new ApplicationException("Report does not belong to the logged in user - Security violation");

		if (!Page.IsPostBack)
		{
			populateReportPartList();

			//Confirm deletes
			JavaScriptBlock.ConfirmClick(cmdDelete, "Are you sure you want to delete the selected report part from this report?");
		}
	}

	private void cmdDelete_Click(object sender, EventArgs e)
	{
		CustomReportComponent part;
		int deleteIndex;

		part = getSelectedPart();
		if (part == null)
			return;

		_db.ORManager.Delete(part);

		deleteIndex = lstReportParts.SelectedIndex;
		lstReportParts.Items.Remove(lstReportParts.SelectedItem);

		lstReportParts.SelectedIndex = ListUtilities.GetItemIndexAfterDelete(lstReportParts.Items.Count, deleteIndex);
	}

	private void cmdAdd_Click(object sender, EventArgs e)
	{
		CustomReportComponent part;
		ListItem newListItem;

		part = new CustomReportComponent();
		part.CustomReport = _report;
		//Default to a site summary
		part.ComponentType = _db.ORManager.Get<CustomReportComponentType>(2);
		part.Url = siteList.GetSelectedSite();

		_db.ORManager.SaveOrUpdate(part);

		newListItem = new ListItem(part.ComponentType.Name, part.Id.ToString());
		lstReportParts.Items.Add(newListItem);
		//Select the newly added item
		lstReportParts.SelectedIndex = lstReportParts.Items.IndexOf(newListItem);
		displayPart(part);
	}

	private void initPreviewLink()
	{
		string url;

		url = string.Format("Report-Viewer.aspx?{0}={1}", Members_Reports_Custom_Reports_Report_Viewer.PARAM_REPORT_ID,
		                    _report.Id);
		lnkPreview.NavigateUrl = url;
	}

	private CustomReportComponent getSelectedReportPart()
	{
		int selectedId;

		if (lstReportParts.SelectedIndex == -1)
			return null;

		selectedId = int.Parse(lstReportParts.SelectedValue);

		return _db.ORManager.Get<CustomReportComponent>(selectedId);
	}

	private void cmdSave_Click(object sender, EventArgs e)
	{
		CustomReportComponent part;
		reportPartTypes partType;

		part = getSelectedReportPart();
		if (part == null)
		{
			_log.Warn("'Save' was pressed, but a part isn't selected");
			return;
		}

		partType = getSelectedPartType();

		//Clear the current properties, and reset only the ones we need to use now
		part.ComponentType = null;
		part.ConfiguredDatasource = null;
		part.DatasourceSubType = null;
		part.SavedReport = null;
		part.Url = null;

		part.CustomReport = _report;

		switch (partType)
		{
			case reportPartTypes.SavedTrend:
				part.ComponentType = _db.ORManager.Get<CustomReportComponentType>(1);
				part.SavedReport = _db.ORManager.Get<SavedReport>(int.Parse(ddlSavedTrends.SelectedValue));
				break;
			case reportPartTypes.SiteSummary:
				part.ComponentType = _db.ORManager.Get<CustomReportComponentType>(2);
				part.Url = siteList.GetSelectedSite();
				break;
		}

		_db.ORManager.SaveOrUpdate(part);
	}

	private void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
	{
		showAndPopulateStep2(null);
	}

	private void lstReportParts_SelectedIndexChanged(object sender, EventArgs e)
	{
		displayPart(getSelectedPart());
	}

	private reportPartTypes getSelectedPartType()
	{
		int intVal;

		intVal = int.Parse(ddlReportType.SelectedValue);
		return (reportPartTypes) intVal;
	}

	/// <summary>
	///		Displays the panel to select the information for step 2, and
	///		populates that panel.
	/// </summary>
	private void showAndPopulateStep2(CustomReportComponent part)
	{
		reportPartTypes partType;

		pnlSavedTrends.Visible = false;
		pnlDataType.Visible = false;
		pnlSiteSelection.Visible = false;

		partType = getSelectedPartType();

		switch (partType)
		{
			case reportPartTypes.SavedTrend:
				showAndPopulateSavedTrends(part);
				break;
			case reportPartTypes.SiteSummary:
				showAndPopulateSiteList(part);
				break;
		}
	}

	private void showAndPopulateSavedTrends(CustomReportComponent part)
	{
		IList<SavedReport> reports;
		ICriteria criteria;

		pnlSavedTrends.Visible = true;

		if (ddlSavedTrends.Items.Count == 0)
		{
			criteria = _db.ORManager.Session.CreateCriteria(typeof (SavedReport)).Add(Expression.Eq("UserId", _userId));
			reports = criteria.List<SavedReport>();

			ddlSavedTrends.DataSource = reports;
			ddlSavedTrends.DataTextField = "Name";
			ddlSavedTrends.DataValueField = "Id";
			ddlSavedTrends.DataBind();
		}

		//Select the correct saved report
		if (part != null && part.SavedReport != null)
			ddlSavedTrends.SelectedValue = part.SavedReport.Id.ToString();
	}

	private void showAndPopulateSiteList(CustomReportComponent part)
	{
		pnlSiteSelection.Visible = true;
		siteList.PopulatePageList();

		if (part != null && part.Url != null)
			siteList.SetSelectedSiteId(part.Url.Id);
	}

	private void parseParameters()
	{
		string reportIdString;

		reportIdString = Request.QueryString[PARAMETER_CUSTOM_REPORT_ID];
		if (string.IsNullOrEmpty(reportIdString))
			throw new ApplicationException(PARAMETER_CUSTOM_REPORT_ID + " was not specified");

		_customReportId = int.Parse(reportIdString);
	}

	private CustomReportComponent getSelectedPart()
	{
		int selectedId;

		if (lstReportParts.SelectedIndex == -1)
			return null;

		selectedId = int.Parse(lstReportParts.SelectedValue);
		return _db.ORManager.Get<CustomReportComponent>(selectedId);
	}

	private void displayPart(CustomReportComponent part)
	{
		if (part == null)
		{
		}
		else
		{
			//Display the correct type
			ddlReportType.SelectedValue = part.ComponentType.Id.ToString();
			showAndPopulateStep2(part);
		}
	}

	private void populateReportPartList()
	{
		lstReportParts.Items.Clear();
		foreach (CustomReportComponent currPart in _report.ReportComponents)
			lstReportParts.Items.Add(new ListItem(currPart.ComponentType.Name, currPart.Id.ToString()));

		if (lstReportParts.Items.Count > 0)
			lstReportParts.SelectedIndex = 0;

		displayPart(getSelectedPart());
	}

	#region Nested type: reportPartTypes

	private enum reportPartTypes
	{
		SavedTrend = 1,
		SiteSummary = 2
	}

	#endregion
}