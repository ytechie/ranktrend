using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;
using Rt.Framework.Applications.InteractiveReport;

namespace Rt.Framework.Services.ReportEngine
{
	/// <summary>
	///		Creates a <see cref="GeneratedReportPart" /> objects for the specified
	///		<see cref="CustomReportComponent" />. This is primarily used to generate
	///		custom reports for the user.
	/// </summary>
	public class ReportPartGenerator
	{
		Database _db;

		public ReportPartGenerator(Database db)
		{
			_db = db;
		}

		/// <summary>
		///		This method generates report parts using a loaded
		///		<see cref="CustomReportComponent" />
		/// </summary>
		/// <param name="reportPart">
		///		The <see cref="CustomReportComponent" /> to generate.
		/// </param>
		/// <returns>
		///		A <see cref="GeneratedReportPart" /> that can be used to
		///		display the part to the user, or email to them.
		/// </returns>
		public GeneratedReportPart GenerateReport(CustomReportComponent reportPart)
		{
			if (reportPart.SavedReport != null)
				return GenerateReport(reportPart.ComponentType.Id.Value, reportPart.SavedReport);
			else if (reportPart.ComponentType.Id == 2)
				return generateSiteSummary(reportPart);
			else
				throw new NotImplementedException();
		}

		public GeneratedReportPart GenerateReport(int componentTypeId, int savedReportId)
		{
			SavedReport savedReport;

			savedReport = _db.ORManager.Get<SavedReport>(savedReportId);

			return GenerateReport(componentTypeId, savedReport);
		}

		public GeneratedReportPart GenerateReport(int componentTypeId, SavedReport savedReport)
		{
			switch (componentTypeId)
			{
				//Custom trend report
				case 1:
					return generateTrendReport(savedReport);
				default:
					return null;
			}
		}

		private GeneratedReportPart generateTrendReport(SavedReport savedReport)
		{
			RankChartGenerator generator;
			RankChartParameters rcp;
			byte[] bytes;
			GeneratedReportPart grp;

			rcp = RankChartParameters.XmlDeserialize(savedReport.XmlData);

			generator = new RankChartGenerator(_db, rcp, savedReport.Name);
			bytes = generator.GenerateChart();

			grp = new GeneratedReportPart();
			grp.Bytes = bytes;

			return grp;
		}

		private GeneratedReportPart generateSiteSummary(Rt.Framework.Components.CustomReportComponent reportPart)
		{
			GeneratedReportPart part;

			Rt.Framework.Services.ReportEngine.ReportControls.SiteSummaryTable summary;

			summary = new Rt.Framework.Services.ReportEngine.ReportControls.SiteSummaryTable();
			summary.setDatasource(_db, reportPart.Url.Id);

			part = new GeneratedReportPart();
			part.Html = getHtmlFromControlRender(summary);

			return part;
		}

		private string getHtmlFromControlRender(System.Web.UI.Control control)
		{
			StringBuilder sb;
			System.IO.StringWriter sw;
			System.Web.UI.HtmlTextWriter writer;

			sb = new StringBuilder();
			using (sw = new System.IO.StringWriter(sb))
			{
				using (writer = new System.Web.UI.HtmlTextWriter(sw))
				{
					control.RenderControl(writer);
				}
			}

			return sb.ToString();
		}
	}
}
