using System;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Administrators_Datasource_Data_Queue_View_Queue_Item : Page
{
	private Database _db;
	private int _queueId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();
		getParameters();
		displayContent();
	}

	private void getParameters()
	{
		_queueId = int.Parse(Request.QueryString["Id"]);
	}

	private void displayContent()
	{
		DatasourceDataQueueItem queueItem;

		queueItem = _db.ORManager.Get<DatasourceDataQueueItem>(_queueId);

		Response.Write(queueItem.Data);
		Response.End();
	}
}