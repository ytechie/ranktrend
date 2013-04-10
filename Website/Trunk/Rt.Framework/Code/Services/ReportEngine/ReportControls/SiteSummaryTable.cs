using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;

namespace Rt.Framework.Services.ReportEngine.ReportControls
{
	//TODO: Localize time zones!!!

	/// <summary>
	///		
	/// </summary>
	public class SiteSummaryTable : GridView
	{
		Database _db;
		int _siteId;

		public SiteSummaryTable()
		{
			initGridColumns();
			initGridStyles();
		}

		public void setDatasource(Database db, int siteId)
		{
			_db = db;
			_siteId = siteId;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			Literal title;
			UrlClass siteInfo;

			if (_db == null)
				return;

			//Load the site information
			siteInfo = _db.ORManager.Get<UrlClass>(_siteId);

			title = new Literal();
			title.Text = string.Format("<b>Information for {0}</b><br />", siteInfo.Url);

			title.RenderControl(writer);
			
			DataSource = _db.Report_SummaryView(_siteId);
			DataBind();

			base.Render(writer);
		}

		private void initGridStyles()
		{
			AutoGenerateColumns = false;
			CssClass = "TabularTable";
			RowStyle.CssClass = "TabularTableItem";
			AlternatingRowStyle.CssClass = "TabularTableAltItem";
			Style["width"] = "100%";
		}

		private void initGridColumns()
		{
			BoundField currentField;

			currentField = new BoundField();
			currentField.DataField = "DatasourceName";
			currentField.HeaderText = "Datasource";
			Columns.Add(currentField);

			currentField = new BoundField();
			currentField.DataField = "LastTimestamp";
			currentField.HeaderText = "Last Read Time";
			Columns.Add(currentField);

			currentField = new BoundField();
			currentField.DataField = "LastValue";
			currentField.HeaderText = "Last Value";
			currentField.DataFormatString = "{0:0.##}";
			currentField.HtmlEncode = false;
			Columns.Add(currentField);

			currentField = new BoundField();
			currentField.DataField = "CurrentWeekAvg";
			currentField.HeaderText = "7 Day Avg";
			currentField.DataFormatString = "{0:0.##}";
			currentField.HtmlEncode = false;
			Columns.Add(currentField);

			currentField = new BoundField();
			currentField.DataField = "Current30DayAvg";
			currentField.HeaderText = "30 Day Avg";
			currentField.DataFormatString = "{0:0.##}";
			currentField.HtmlEncode = false;
			Columns.Add(currentField);

			currentField = new BoundField();
			currentField.DataField = "OverallAvg";
			currentField.HeaderText = "Overall Average";
			currentField.DataFormatString = "{0:0.##}";
			currentField.HtmlEncode = false;
			Columns.Add(currentField);

			currentField = new BoundField();
			currentField.DataField = "StandardDeviation";
			currentField.HeaderText = "Std. Dev.";
			currentField.DataFormatString = "{0:0.##}";
			currentField.HtmlEncode = false;
			Columns.Add(currentField);
		}
	}
}
