using System;
using System.Web.UI;
using Rt.Framework.Applications.InteractiveReport;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Interactive_Report_Embedded_Image : Page
{
	public const string QUERY_REPORT_ID = "rid";

	private Database _db;
	private int reportId;

	protected void Page_Load(object sender, EventArgs e)
	{
		string reportIdString;
		bool reportIdParseable;

		reportIdString = Request.QueryString[QUERY_REPORT_ID];

		if (reportIdString == null)
			return;

		reportIdParseable = int.TryParse(reportIdString, out reportId);

		if (!reportIdParseable)
			return;

		//Don't create the database connection until the request is validated
		_db = Global.GetDbConnection();

		renderReport();
	}

	private void renderReport()
	{
		RankChartGenerator generator;
		RankChartParameters rcp;
		SavedReport settings;

		settings = _db.ORManager.Get<SavedReport>(reportId);

		//Make sure the report is public
		if (!settings.PublicViewable)
			return;

		rcp = RankChartParameters.XmlDeserialize(settings.XmlData);

		generator = new RankChartGenerator(_db, rcp);
		generator.GenerateChart(Response);
	}
}