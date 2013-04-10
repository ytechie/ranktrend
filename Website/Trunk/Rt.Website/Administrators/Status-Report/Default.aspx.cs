using System;
using System.Data;
using System.Web.UI;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Administrators_Status_Report_Default : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		if (!Page.IsPostBack)
		{
			populateReportData();
		}
	}

	private void populateReportData()
	{
		DataSet ds;

		ds = _db.GetAdministrativeReport();

		dgGeneral.DataSource = ds.Tables[0];
		dgGeneral.DataBind();

		dgDatasources.DataSource = ds.Tables[1];
		dgDatasources.DataBind();

		dgDbStats1.DataSource = ds.Tables[2];
		dgDbStats1.DataBind();

		dgDbStats2.DataSource = ds.Tables[3];
		dgDbStats2.DataBind();
	}
}