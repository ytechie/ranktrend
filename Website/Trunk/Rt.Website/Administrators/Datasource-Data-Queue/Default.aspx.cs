using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services.DataEngine;
using Rt.Website;
using YTech.General.Web.JSProxy;
using IDataSource=Rt.Framework.Services.DataEngine.IDataSource;

public partial class Administrators_Datasource_Data_Queue_Default : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		AjaxServerMethodHandler.CheckAjaxCall(this);

		if (!Page.IsPostBack)
			populateQueueList();

		Global.AddCommonJavaScript(this);
		Page.ClientScript.RegisterClientScriptInclude("local", ResolveUrl("Default-Script.js"));
	}

	[JSProxy]
	public void DeleteQueueItem(int queueId)
	{
		DatasourceDataQueueItem queueItem;

		queueItem = _db.ORManager.Get<DatasourceDataQueueItem>(queueId);
		_db.ORManager.Delete(queueItem);
	}

	private void populateQueueList()
	{
		DataTable dt;

		dt = _db.GetQueuedDatasourceData();

		dvQueue.RowDataBound += dvQueue_RowDataBound;
		dvQueue.DataSource = dt;
		dvQueue.DataBind();
	}

	private void dvQueue_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			Button lb;
			int rowKey;

			rowKey = getRowKey(e.Row.RowIndex);

			//Add the client side code for the delete button
			lb = (Button) e.Row.FindControl("cmdDelete");
			lb.Attributes["onclick"] = string.Format("javascript:return deleteRecord(this, {0});", rowKey);

			//Now find the reprocess button and add it's client-side code
			lb = (Button) e.Row.FindControl("cmdReprocess");
			lb.Attributes["onclick"] = string.Format("javascript:return reprocessRecord(this, {0});", rowKey);
		}
	}

	private int getRowKey(int rowIndex)
	{
		DataTable dt;

		dt = (DataTable) dvQueue.DataSource;
		return (int) dt.Rows[rowIndex]["Id"];
	}

	protected void Reprocess_Click(object s, EventArgs e)
	{
		Button button;
		int queueId;

		button = (Button) s;
		queueId = int.Parse(button.CommandArgument);

		ReprocessItem(queueId);
	}

	[JSProxy]
	public bool ReprocessItem(int queueId)
	{
		DatasourceManager datasourceManager;
		DatasourceDataQueueItem queueItem;
		RawDataValue rawData;
		ConfiguredDatasource cds;
		IDataSource currentDatasource;
		DatasourceParameterType[] parameterTypes;

		queueItem = _db.ORManager.Get<DatasourceDataQueueItem>(queueId);
		rawData = _db.ORManager.Get<RawDataValue>(queueItem.RawDataId);
		cds = _db.ORManager.Get<ConfiguredDatasource>(rawData.ConfiguredDatasourceId);

		datasourceManager = new DatasourceManager();

		currentDatasource = datasourceManager.GetDatasource(cds.DatasourceType.Id);

		//Load the parameter types
		parameterTypes = _db.GetParameterTypesForDatasource(cds.DatasourceType.Id);

		//Set up the datasource
		cds.InitializeDatasource(currentDatasource, parameterTypes);

		//Fake the web response
		SerializableWebResponse resp;
		resp = new SerializableWebResponse();
		resp.Content = queueItem.Data;
		currentDatasource.SetResponse(resp);

		RawDataValue[] values;
		values = currentDatasource.Values;

		foreach (RawDataValue currValue in values)
		{
			if (!currValue.Success || currValue.Fuzzy)
				return false;
		}

		//Save the new values
		foreach (RawDataValue currValue in values)
		{
			currValue.ConfiguredDatasourceId = cds.Id.Value;
			_db.BulkInsertRawData(values);
		}

		//Delete the extra state data
		_db.ORManager.Delete(queueItem);

		//If we get here, it must have worked!
		return true;
	}

	protected void Delete_Click(object s, EventArgs e)
	{
		Button button;
		int queueId;

		button = (Button) s;
		queueId = int.Parse(button.CommandArgument);

		//This does seem kind if stupid, but it's fine for now
		DeleteQueueItem(queueId);

		populateQueueList();
	}
}