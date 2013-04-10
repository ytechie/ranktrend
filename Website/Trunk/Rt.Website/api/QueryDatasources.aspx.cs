using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class api_QueryDatasources : Page
{
	private Database _db;
	private int _urlId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();
		getParameters();

		Response.Clear();
		Response.Write(getXml());
		Response.End();
	}

	private void getParameters()
	{
		_urlId = int.Parse(Request.QueryString["urlId"]);
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

		writeDatasources(xml);

		xml.WriteEndDocument();

		return sb.ToString();
	}

	private void writeDatasources(XmlWriter xml)
	{
		IList<ConfiguredDatasource> datasources;

		datasources = getDatasources();

		xml.WriteStartElement("datasources");

		foreach (ConfiguredDatasource cd in datasources)
		{
			xml.WriteStartElement("datasource");

			xml.WriteAttributeString("Id", cd.Id.ToString());
			xml.WriteAttributeString("Name", cd.Name);
			xml.WriteAttributeString("plot", "0");
			//xml.WriteAttributeString("url", cd.Url.Url);

			xml.WriteEndElement();
		}

		xml.WriteEndElement();
	}

	private IList<ConfiguredDatasource> getDatasources()
	{
		DataTable dt;
		var datasources = new List<ConfiguredDatasource>();

		dt = _db.Ds_GetPageDatasourceList(_urlId, true);
		foreach (DataRow currRow in dt.Rows)
		{
			var cd = new ConfiguredDatasource();
			cd.Id = (int) currRow["Id"];
			cd.Name = (string) currRow["DisplayName"];

			datasources.Add(cd);
		}

		return datasources;
	}
}