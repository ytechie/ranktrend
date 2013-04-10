using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class api_QueryData : Page
{
	private string[] _datasourceKeys;

	private Database _db;
	private DateTime _end;
	private DateTime _start;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();
		getParameters();

		Response.Clear();
		Response.Write(getXml());
		Response.End();
	}

	private string getXml()
	{
		XmlTextWriter xml;
		StringWriter sw;
		StringBuilder sb;

		sb = new StringBuilder();
		sw = new StringWriter(sb);
		xml = new XmlTextWriter(sw);

		xml.WriteStartDocument();

		writeDatas(xml);

		xml.WriteEndDocument();

		return sb.ToString();
	}

	private void writeDatas(XmlWriter xml)
	{
		Dictionary<string, DataTable> dataTables;

		dataTables = queryData();

		//In honor of Adam
		xml.WriteStartElement("datas");

		foreach (string dataKey in dataTables.Keys)
			writeData(xml, dataKey, dataTables[dataKey]);

		xml.WriteEndElement();
	}

	private void writeData(XmlWriter xml, string dataKey, DataTable data)
	{
		xml.WriteStartElement("data");

		xml.WriteAttributeString("datasourceKey", dataKey);
		xml.WriteElementString("points", getDataString(data));

		xml.WriteEndElement();
	}

	private void getParameters()
	{
		string paramValue;

		_start = DateTime.Parse(Request.QueryString["start"]);
		_end = DateTime.Parse(Request.QueryString["end"]);
		paramValue = Request.QueryString["datasourceIds"];
		_datasourceKeys = paramValue.Split(",".ToCharArray());
	}

	private Dictionary<string, DataTable> queryData()
	{
		Dictionary<string, DataTable> dataTables;

		//TODO: Query all of the datasources in one batch for performance

		dataTables = new Dictionary<string, DataTable>();
		foreach (string currDatasourceKey in _datasourceKeys)
		{
			DataTable dt;
			int datasourceId;
			int? datasourceSubId;

			datasourceId = int.Parse(currDatasourceKey);
			datasourceSubId = null; //TODO: support datasource sub types

			dt = _db.QueryRawData(_start, _end, datasourceId, datasourceSubId);
			dataTables.Add(datasourceId.ToString(), dt);
		}

		return dataTables;
	}

	//private void writeData()
	//{
	//  DataTable dt = _db.QueryRawData(_start, _end, _datasourceId, _datasourceSubId);
	//  StringBuilder sb;
	//  DateTime baseTime;

	//  baseTime = new DateTime(1970, 1, 1);
	//  sb = new StringBuilder();
	//  foreach(DataRow currRow in dt.Rows)
	//  {
	//    DateTime timestamp = (DateTime)currRow["Timestamp"];
	//    if (currRow["FloatValue"] != DBNull.Value) //TODO: Figure out how to send nulls
	//    {
	//      double value = (double) currRow["FloatValue"];

	//      if (sb.Length > 0)
	//        sb.Append("|");
	//      sb.Append(timestamp.Subtract(baseTime).TotalMilliseconds);
	//      sb.Append(",");
	//      sb.Append(value.ToString());
	//    }
	//  }

	//  Response.Clear();
	//  Response.Write(sb.ToString());
	//  Response.End();
	//}

	private static string getDataString(DataTable data)
	{
		StringBuilder sb;
		DateTime baseTime;

		baseTime = new DateTime(1970, 1, 1);
		sb = new StringBuilder();
		foreach (DataRow currRow in data.Rows)
		{
			var timestamp = (DateTime) currRow["Timestamp"];
			if (currRow["FloatValue"] != DBNull.Value) //TODO: Figure out how to send nulls
			{
				var value = (double) currRow["FloatValue"];

				if (sb.Length > 0)
					sb.Append("|");
				sb.Append(timestamp.Subtract(baseTime).TotalMilliseconds);
				sb.Append(",");
				sb.Append(value.ToString());
			}
		}

		return sb.ToString();
	}
}