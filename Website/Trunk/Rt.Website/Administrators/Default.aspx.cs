using System;
using System.Reflection;
using System.Web.UI;
using log4net;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services.DataEngine;
using Rt.Website;

public partial class Administrators_Default : Page
{
	/// <summary>
	///		Declare and create our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void updateDatasources(object source, EventArgs evt)
	{
		Database db = Global.GetDbConnection();
		_log.Debug("User clicked on the 'Update All Datasources' button");
		LocalDatasourceExecutor.RunPendingJobs(db);
	}
}