using System;
using System.Web.UI;
using Rt.Framework.Applications.InteractiveReport;
using Rt.Framework.CommonControls.Web;
using Rt.Framework.Components;
using Rt.Website;

public partial class Members_Interactive_Report_OverviewTab : UserControl
{
	public DateRangeSelector DateSelector
	{
		get { return dateSelector; }
	}

	public string ReportName
	{
		get { return txtReportName.Text; }
		/*set { txtReportName.Text = value; }*/
	}

	public RankChartParameters.ChartSizes ChartSize
	{
		get { return (RankChartParameters.ChartSizes) int.Parse(sizeSelector.SelectedValue); }
		set { sizeSelector.SelectedValue = value.ToString("d"); }
	}

	public DateTime ReportLastSaved
	{
		set { Global.ConvertToUserTime(value).ToString(); }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	public void DisplaySettings(SavedReport settings)
	{
		//if (settings == null)
		//{
		//  txtReportName.Text = string.Format("Report Created {0}", Rt.Website.Global.ConvertToUserTime(DateTime.UtcNow).ToString());
		//  lblReportCreated.Text = "N/A";
		//  lblReportLastSaved.Text = "Never";
		//}
		//else
		//{
		txtReportName.Text = settings.Name;
		lblReportCreated.Text = Global.ConvertToUserTime(settings.Created).ToString();
		if (settings.LastSaved == null)
			lblReportLastSaved.Text = "Never";
		else
			lblReportLastSaved.Text = Global.ConvertToUserTime(settings.LastSaved.Value).ToString();
		//}
	}
}