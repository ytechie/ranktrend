using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Reports_Custom_Reports_Report_Editor : Page
{
	private Database _db;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_userId = (Guid) Membership.GetUser().ProviderUserKey;
		_db = Global.GetDbConnection();

		lstReports.SelectedIndexChanged += lstReports_SelectedIndexChanged;
		cmdAdd.Click += cmdAdd_Click;
		cmdSave.Click += cmdSave_Click;

		if (!Page.IsPostBack)
		{
			populateReportList();
			displayReportDetails(getSelectedReport());
		}
	}

	private void lstReports_SelectedIndexChanged(object sender, EventArgs e)
	{
		displayReportDetails(getSelectedReport());
	}

	private CustomReport getSelectedReport()
	{
		int selectedId;

		if (lstReports.SelectedIndex == -1)
			return null;

		selectedId = int.Parse(lstReports.SelectedValue);

		return _db.ORManager.Get<CustomReport>(selectedId);
	}

	private void displayReportDetails(CustomReport rpt)
	{
		if (rpt == null)
		{
			txtName.Enabled = false;
			txtName.Text = "";
			txtDescription.Enabled = false;
			txtDescription.Text = "";
			ddlEmailInterval.Enabled = false;
			ddlEmailInterval.SelectedIndex = 0;
			cmdSave.Enabled = false;
			blReportcomponents.Items.Clear();
			lnkPreview.Visible = false;
		}
		else
		{
			txtName.Enabled = true;
			txtName.Text = rpt.Name;
			txtDescription.Enabled = true;
			txtDescription.Text = rpt.Description;
			ddlEmailInterval.Enabled = true;
			ddlEmailInterval.SelectedValue = rpt.EmailIntervalDays.ToString();
			cmdSave.Enabled = true;

			blReportcomponents.Items.Clear();
			//Populate the report component list
			foreach (CustomReportComponent currComponent in rpt.ReportComponents)
			{
				blReportcomponents.Items.Add(currComponent.ComponentType.Name);
			}

			if (blReportcomponents.Items.Count > 0)
			{
				lnkPreview.Visible = true;
				initPreviewLink(rpt.Id.Value);
			}
			else
			{
				lnkPreview.Visible = false;
			}

			lnkEditReportComponents.NavigateUrl =
				string.Format("~/Members/Reports/Custom-Reports/Report-Component-Editor.aspx?Custom-Report-Id={0}", rpt.Id);
		}
	}

	private void initPreviewLink(int reportId)
	{
		string url;

		url = string.Format("Report-Viewer.aspx?{0}={1}", Members_Reports_Custom_Reports_Report_Viewer.PARAM_REPORT_ID,
		                    reportId);
		lnkPreview.NavigateUrl = url;
	}

	private void cmdAdd_Click(object sender, EventArgs e)
	{
		CustomReport newRpt;

		newRpt = new CustomReport();
		newRpt.Name = "Untitled Custom Report";
		newRpt.Description = string.Empty;
		newRpt.EmailIntervalDays = 0;
		newRpt.UserId = _userId;

		_db.ORManager.SaveOrUpdate(newRpt);

		lstReports.Items.Add(new ListItem(newRpt.Name, newRpt.Id.ToString()));
	}

	private void cmdSave_Click(object sender, EventArgs e)
	{
		CustomReport rpt;

		rpt = getSelectedReport();

		if (txtName.Text != rpt.Name)
		{
			lstReports.SelectedItem.Text = txtName.Text;
			rpt.Name = txtName.Text;
		}

		rpt.Description = txtDescription.Text;
		rpt.EmailIntervalDays = int.Parse(ddlEmailInterval.SelectedValue);
		//TODO: get the database server time
		rpt.LastSaved = DateTime.UtcNow;

		_db.ORManager.SaveOrUpdate(rpt);
	}

	private void populateReportList()
	{
		ICriteria criteria;
		IList<CustomReport> reports;

		criteria = _db.ORManager.Session.CreateCriteria(typeof (CustomReport)).Add(Expression.Eq("UserId", _userId));
		reports = criteria.List<CustomReport>();

		lstReports.DataSource = reports;
		lstReports.DataTextField = "Name";
		lstReports.DataValueField = "Id";
		lstReports.DataBind();

		if (lstReports.Items.Count > 0)
			lstReports.SelectedIndex = 0;
	}
}