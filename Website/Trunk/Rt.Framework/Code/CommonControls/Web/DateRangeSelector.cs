using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using eWorld.UI;
using Rt.Framework.Applications.InteractiveReport;

namespace Rt.Framework.CommonControls.Web
{
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:DateRangeSelector runat=server></{0}:DateRangeSelector>")]
	public class DateRangeSelector : WebControl
	{
		#region Child Controls

		private HtmlGenericControl _dateRangeLabel;
		private DropDownList _ddlDateRange;
		private CalendarPopup _endCalendar;
		private HtmlGenericControl _endLabel;
		private CalendarPopup _startCalendar;
		private HtmlGenericControl _startLabel;

		#endregion

		#region DateRanges enum

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		///		These are assigned values so that they can be guaranteed
		///		not to change if we add or remove ranges.
		/// </remarks>
		public enum DateRanges
		{
			Last30Days = 1,
			Last60Days = 2,
			Last90Days = 3,
			Last6Months = 4,
			Custom = 5
		}

		#endregion

		protected override void OnInit(EventArgs e)
		{
			Load += new EventHandler(DateRangeSelector_Load);

			base.OnInit(e);

			createChildControls();

			if (!Page.IsPostBack)
				populateDateRanges();
		}

		private void DateRangeSelector_Load(object sender, EventArgs e)
		{
			_ddlDateRange.SelectedIndexChanged += new EventHandler(ddlDateRange_SelectedIndexChanged);
		}

		private void createChildControls()
		{
			Literal br;

			_dateRangeLabel = new HtmlGenericControl("span");
			_dateRangeLabel.Attributes["class"] = "ir_Overview_Label";
			_dateRangeLabel.InnerText = "Date Range:";
			Controls.Add(_dateRangeLabel);

			_ddlDateRange = new DropDownList();
			_ddlDateRange.AutoPostBack = true;
			Controls.Add(_ddlDateRange);

			br = new Literal();
			br.Text = "<br />";
			Controls.Add(br);

			_startLabel = new HtmlGenericControl("span");
			_startLabel.Attributes["class"] = "ir_Overview_Label";
			_startLabel.InnerText = "Start:";
			Controls.Add(_startLabel);

			_startCalendar = new CalendarPopup();
			_startCalendar.ControlDisplay = DisplayType.TextBoxImage;
			_startCalendar.CalendarWidth = 175;
			_startCalendar.ImageUrl = "~/Images/CalendarIcon.gif";
			Controls.Add(_startCalendar);

			br = new Literal();
			br.Text = "<br />";
			Controls.Add(br);

			_endLabel = new HtmlGenericControl("span");
			_endLabel.Attributes["class"] = "ir_Overview_Label";
			_endLabel.InnerText = "End:";
			Controls.Add(_endLabel);

			_endCalendar = new CalendarPopup();
			_endCalendar.ControlDisplay = DisplayType.TextBoxImage;
			_endCalendar.CalendarWidth = 175;
			_endCalendar.ShowGoToToday = true;
			_endCalendar.ImageUrl = "~/Images/CalendarIcon.gif";
			Controls.Add(_endCalendar);

			base.CreateChildControls();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			foreach (Control currChild in Controls)
				currChild.RenderControl(writer);
		}

		private void populateDateRanges()
		{
			_ddlDateRange.Items.Add(new ListItem("Last 30 Days", ((int) DateRanges.Last30Days).ToString()));
			_ddlDateRange.Items.Add(new ListItem("Last 60 Days", ((int) DateRanges.Last60Days).ToString()));
			_ddlDateRange.Items.Add(new ListItem("Last 90 Days", ((int) DateRanges.Last90Days).ToString()));
			_ddlDateRange.Items.Add(new ListItem("Last 6 Months", ((int) DateRanges.Last6Months).ToString()));
			_ddlDateRange.Items.Add(new ListItem("Custom Range (Use Fields Below)", ((int) DateRanges.Custom).ToString()));

			_ddlDateRange.SelectedIndex = 0;

			ddlDateRange_SelectedIndexChanged(this, null);
		}

		/// <summary>
		///		Enables or disables the custom date range fields, and updates
		///		the date pickers based on the selected date range.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ddlDateRange_SelectedIndexChanged(object sender, EventArgs e)
		{
			DateRanges dateRangeEnum;
			DateTime start, end;

			dateRangeEnum = (DateRanges) int.Parse(_ddlDateRange.Text);

			if (dateRangeEnum == DateRanges.Custom)
			{
				_startCalendar.Enabled = true;
				_endCalendar.Enabled = true;
			}
			else
			{
				_startCalendar.Enabled = false;
				_endCalendar.Enabled = false;
			}

			//Set the date pickers to the newly displayed range
			GetTimeRange(out start, out end);
			_startCalendar.SelectedDate = start;
			_endCalendar.SelectedDate = end;
		}

		public void GetTimeRange(out DateTime startDate, out DateTime endDate)
		{
			DateRanges dateRangeEnum;

			GetTimeRange(out startDate, out endDate, out dateRangeEnum);
		}

		/// <summary>
		///		Gets the time range that is currently selected in the control.
		/// </summary>
		/// <param name="startDate">
		///		The <see cref="DateTime" /> of the start of the selected range.
		/// </param>
		/// <param name="endDate">
		///		The <see cref="DateTime" /> of the end of the selected range.
		/// </param>
		/// <param name="rangeType">
		///		The <see cref="DateRanges" /> value that defines the type of
		///		range being returned. It can either be a predefined range, or
		///		a custom range.
		/// </param>
		public void GetTimeRange(out DateTime startDate, out DateTime endDate, out DateRanges rangeType)
		{
			rangeType = (DateRanges) int.Parse(_ddlDateRange.Text);

			//If it's custom, we need to read the user input
			if (rangeType == DateRanges.Custom)
			{
				startDate = _startCalendar.SelectedDate.Date;
				endDate = _endCalendar.SelectedDate.Date;
				return;
			}

			GetDateRangeByType(rangeType, out startDate, out endDate);
		}

		public void SetTimeRange(DateTime startDate, DateTime endDate, DateRanges rangeType)
		{
			ListItem rangeMatch;

			//Determine which list item matches the range type
			rangeMatch = _ddlDateRange.Items.FindByValue(((int) rangeType).ToString());

			_ddlDateRange.SelectedIndex = _ddlDateRange.Items.IndexOf(rangeMatch);

			//If it's not a custom range, calculate the dates
			if (rangeType != DateRanges.Custom)
				GetDateRangeByType(rangeType, out startDate, out endDate);

			_startCalendar.SelectedDate = startDate;
			_endCalendar.SelectedDate = endDate;
		}

		public void SetTimeRange(DateTime startDate, DateTime endDate)
		{
			//Switch to custom mode
			_ddlDateRange.SelectedValue = ((int) DateRanges.Custom).ToString();

			_startCalendar.SelectedDate = startDate.Date;
			_endCalendar.SelectedDate = endDate.Date;

			_startCalendar.Enabled = true;
			_endCalendar.Enabled = true;
		}

		public static void GetDateRangeByType(DateRanges rangeType, out DateTime startDate, out DateTime endDate)
		{
			switch (rangeType)
			{
				case DateRanges.Last30Days:
					startDate = DateTime.UtcNow.Date.AddDays(-30);
					endDate = DateTime.UtcNow.Date;
					break;
				case DateRanges.Last60Days:
					startDate = DateTime.UtcNow.Date.AddDays(-60);
					endDate = DateTime.UtcNow.Date;
					break;
				case DateRanges.Last90Days:
					startDate = DateTime.UtcNow.Date.AddDays(-90);
					endDate = DateTime.UtcNow.Date;
					break;
				case DateRanges.Last6Months:
					startDate = DateTime.UtcNow.Date.AddMonths(-6);
					endDate = DateTime.UtcNow.Date;
					break;
				case DateRanges.Custom:
					throw new ApplicationException("Method cannot determine time range when using a custom range");
				default:
					throw new ApplicationException("Unknown enumeration value for date range");
			}
		}

		public void ZoomIn()
		{
			zoom(true);
		}

		public void ZoomOut()
		{
			zoom(false);
		}

		public void ScrollLeft()
		{
			scroll(true);
		}

		public void ScrollRight()
		{
			scroll(false);
		}

		private void zoom(bool zoomIn)
		{
			DateTime start, end;

			GetTimeRange(out start, out end);

			RankChartParameters.ZoomTimeRange(start, end, out start, out end, zoomIn);

			SetTimeRange(start, end);
		}

		private void scroll(bool scrollLeft)
		{
			DateTime start, end;

			GetTimeRange(out start, out end);

			RankChartParameters.ScrollTimeRange(start, end, out start, out end, scrollLeft);

			SetTimeRange(start, end);
		}
	}
}