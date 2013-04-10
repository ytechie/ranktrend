using System;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services.ReportEngine;
using Rt.Website;

public partial class Members_Reports_Custom_Reports_Report_Viewer : Page
{
	public const string PARAM_REPORT_ID = "rid";
	private Database _db;
	private CustomReport _report;
	private Guid _userId;

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		_userId = (Guid) Membership.GetUser().ProviderUserKey;
		_db = Global.GetDbConnection();

		loadParameters();

		generateReport();
	}

	private void loadParameters()
	{
		string reportIdParameter;
		int reportId;

		reportIdParameter = Request.QueryString[PARAM_REPORT_ID];
		if (string.IsNullOrEmpty(reportIdParameter))
			throw new ApplicationException(PARAM_REPORT_ID + " is a required parameter, but was not supplied");

		reportId = int.Parse(reportIdParameter);
		_report = _db.ORManager.Get<CustomReport>(reportId);

		if (_report.UserId != _userId)
			throw new Exception("A non-report owner attempted to access report #" + reportId);
	}

	private void generateReport()
	{
		foreach (CustomReportComponent currPart in _report.ReportComponents)
		{
			if (currPart.ComponentType.IsHtml)
				showHtmlPart(currPart);
			else
				showImagePart(currPart);

			endReportPart();
		}
	}

	private string getImagePartUrl(CustomReportComponent reportPart)
	{
		StringBuilder url;

		url = new StringBuilder();
		url.Append("~/Report-Part.aspx"); //base page
		//Component type
		url.AppendFormat("?{0}={1}", Members_Reports_Custom_Reports_Report_Part.PARAMETER_COMPONENT_TYPE_ID,
		                 reportPart.ComponentType.Id);

		if (reportPart.ConfiguredDatasource != null)
			url.AppendFormat("&{0}={1}", Members_Reports_Custom_Reports_Report_Part.PARAMETER_CONFIGURED_DATASOURCE_ID,
			                 reportPart.ConfiguredDatasource.Id);
		if (reportPart.DatasourceSubType != null)
			url.AppendFormat("&{0}={1}", Members_Reports_Custom_Reports_Report_Part.PARAMETER_CONFIGURED_DATASOURCE_SUBTYPE_ID,
			                 reportPart.DatasourceSubType.Id);
		if (reportPart.SavedReport != null)
			url.AppendFormat("&{0}={1}", Members_Reports_Custom_Reports_Report_Part.PARAMETER_SAVED_REPORT_ID,
			                 reportPart.SavedReport.Id);

		//Append the user ID so that caching is on a per-user basis
		url.AppendFormat("&{0}={1}", "cache", _userId);

		return url.ToString();
	}

	private void showHtmlPart(CustomReportComponent reportPart)
	{
		Literal htmlControl;
		ReportPartGenerator generator;
		GeneratedReportPart part;

		generator = new ReportPartGenerator(_db);
		part = generator.GenerateReport(reportPart);

		htmlControl = new Literal();
		htmlControl.Text = part.Html;

		reportPlaceholder.Controls.Add(htmlControl);
	}

	private void showImagePart(CustomReportComponent reportPart)
	{
		string imageUrl;
		Image image;

		imageUrl = getImagePartUrl(reportPart);

		image = new Image();
		image.ImageUrl = imageUrl;

		reportPlaceholder.Controls.Add(image);
	}

	/// <summary>
	///		Writes the HTML that should come after every report part.
	/// </summary>
	private void endReportPart()
	{
		Literal br;

		br = new Literal();
		br.Text = "<br /><br />\n";

		reportPlaceholder.Controls.Add(br);
	}
}