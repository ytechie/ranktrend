using System;
using System.Web.UI;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services.ReportEngine;
using Rt.Website;

public partial class Members_Reports_Custom_Reports_Report_Part : Page
{
	public const string PARAMETER_COMPONENT_TYPE_ID = "tid";
	public const string PARAMETER_CONFIGURED_DATASOURCE_ID = "cdid";
	public const string PARAMETER_CONFIGURED_DATASOURCE_SUBTYPE_ID = "cdstid";
	public const string PARAMETER_SAVED_REPORT_ID = "srid";
	private int _componentTypeId;
	private int? _configuredDatasourceId;
	private int? _datasourceSubTypeId;
	private Database _db;
	private ReportPartGenerator _generator;
	private int? _savedReportId;

	//TODO: Add support for security on the URL parameters passed over using an HD5 checksum

	protected void Page_Load(object sender, EventArgs e)
	{
		parseQueryParameters();

		_db = Global.GetDbConnection();

		_generator = new ReportPartGenerator(_db);

		//We have to make some broad descisions based on the component
		//type ID.  Of course we want to leave as much logic in the
		//<see cref="ReportPartGenerator" />.
		if (_componentTypeId == 1)
			displaySavedReport();
		else
			throw new NotImplementedException();
	}

	private void parseQueryParameters()
	{
		string currParameter;

		currParameter = Request.QueryString[PARAMETER_COMPONENT_TYPE_ID];
		if (string.IsNullOrEmpty(currParameter))
			throw new ApplicationException(PARAMETER_COMPONENT_TYPE_ID + " is a required parameter");
		else
			_componentTypeId = int.Parse(currParameter);

		currParameter = Request.QueryString[PARAMETER_CONFIGURED_DATASOURCE_ID];
		if (string.IsNullOrEmpty(currParameter))
			_configuredDatasourceId = null;
		else
			_configuredDatasourceId = int.Parse(currParameter);

		currParameter = Request.QueryString[PARAMETER_CONFIGURED_DATASOURCE_SUBTYPE_ID];
		if (string.IsNullOrEmpty(currParameter))
			_datasourceSubTypeId = null;
		else
			_datasourceSubTypeId = int.Parse(currParameter);

		currParameter = Request.QueryString[PARAMETER_SAVED_REPORT_ID];
		if (string.IsNullOrEmpty(currParameter))
			_savedReportId = null;
		else
			_savedReportId = int.Parse(currParameter);
	}

	private void displaySavedReport()
	{
		GeneratedReportPart part;

		if (_savedReportId == null)
			throw new ApplicationException(PARAMETER_SAVED_REPORT_ID + " is a required parameter for type 1 report parts");

		part = _generator.GenerateReport(_componentTypeId, _savedReportId.Value);

		renderImage(part.Bytes);
	}

	private void renderImage(byte[] bytes)
	{
		Response.ContentType = "image/png";
		Response.BinaryWrite(bytes);
		Response.End();
	}
}