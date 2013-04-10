using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using ChartDirector;
using MichaelBrumm.Win32;
using Rt.Framework.CommonControls.Web;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using YTech.General.Charting;

namespace Rt.Framework.Applications.InteractiveReport
{
	public class RankChartGenerator
	{
		private const int BOTTOM_MARGIN = 20;
		private const string CHARTDIRECTOR_LICENSE = "RDST-24PL-BAVB-8BS8-E0F0-9456";

		private const int EVENT_LABEL_LENGTH = 25;
		private const int LAYER_NAME_MAXLENGTH = 35;
		private const int LEFT_MARGIN = 35;
		private const int RIGHT_MARGIN = 5;

		public const string TOOLBAR_KEY_REFRESH = "r";
		public const string TOOLBAR_KEY_SAVE = "s";
		public const string TOOLBAR_KEY_SCROLL_LEFT = "sl";
		public const string TOOLBAR_KEY_SCROLL_RIGHT = "sr";
		public const string TOOLBAR_KEY_ZOOM_IN = "zi";
		public const string TOOLBAR_KEY_ZOOM_OUT = "zo";
		private const int TOP_MARGIN = 20;

		private static readonly Color[] AutoLineColors =
			new Color[] {Color.Blue, Color.Green, Color.Red, Color.Brown, Color.Orange};

		private static readonly int[] CHART_HEIGHT = {215, 250, 500};
		private static readonly int[] CHART_WIDTH = {360, 780, 780};
		private static readonly int[] LEGEND_COLS = {1, 3, 3};
		private XYChart _chart;
		private Database _db;
		private DateTime _end;
		private int _headerHeight;
		private string _noDatasourcesMessage;
		private RankChartParameters _rcp;
		private DateTime _start;
		private TimeZone _timeZone;
		private string _title;
		private string _toolbarImagePath;
		private StringBuilder imageMapAreas;

		public RankChartGenerator(Database db, RankChartParameters rcp) : this(db, rcp, null)
		{
		}

		public RankChartGenerator(Database db, RankChartParameters rcp, string title)
		{
			_db = db;
			_rcp = rcp;
			_title = title;

			//Look up the time zone of the user
			_timeZone = TimeZones.GetTimeZone(_rcp.TimezoneOffsetIndex);

			//Determine the time range
			if (rcp.DateRangeType == DateRangeSelector.DateRanges.Custom)
			{
				_start = _rcp.StartTime;
				_end = _rcp.EndTime;
			}
			else
			{
				DateRangeSelector.GetDateRangeByType(rcp.DateRangeType, out _start, out _end);
			}

			//Add 1 day to the end of the range, so that it ecompasses the whole last day in the range
			_end = _end.AddDays(1);

			_chart = new XYChart(ChartWidth, ChartHeight, 0xeeeeee, 0x000000, 1);
			Chart.setLicenseCode(CHARTDIRECTOR_LICENSE);
		}

		private int ChartHeight
		{
			get { return CHART_HEIGHT[(int) _rcp.ChartSize] + LegendHeight; }
		}

		private int ChartWidth
		{
			get { return CHART_WIDTH[(int) _rcp.ChartSize]; }
		}

		private int BottomMargin
		{
			get { return BOTTOM_MARGIN + LegendHeight; }
		}

		private int LegendHeight
		{
			get { return 15*(int) Math.Ceiling((double) _rcp.Datasources.Length/LegendCols); }
		}

		private int LegendCols
		{
			get { return LEGEND_COLS[(int) _rcp.ChartSize]; }
		}

		public string NoDatasourceMessage
		{
			get { return _noDatasourcesMessage; }
			set { _noDatasourcesMessage = value; }
		}

		public string ToolbarImagePath
		{
			get { return _toolbarImagePath; }
			set { _toolbarImagePath = value; }
		}

		private void generateChart()
		{
			DataTable[] rawDataTables;

			double? yMin = null, yMax = null, y2Min = null, y2Max = null;

			imageMapAreas = new StringBuilder();

			preSetup();

			if (_rcp.Datasources.Length == 0)
			{
				drawMessage(_noDatasourcesMessage);
				finalizeChart();
				return;
			}

			if (_start == _end)
			{
				drawMessage("There is no data available for selected options.");
				finalizeChart();
				return;
			}

			//Get the data from the database
			rawDataTables = retrieveData(_rcp.Datasources, _start, _end);

			//Set the scale to be measured in days
			//Remove the auto-generated labels
			_chart.xAxis().setLinearScale(Chart.CTime(_timeZone.ToLocalTime(_start).Date),
			                              Chart.CTime(_timeZone.ToLocalTime(_end).Date.AddDays(1)), null);

			addTimestampLabels(_start, _end);

			for (int i = 0; i < rawDataTables.Length; i++)
			{
				DBTable tableHelper;
				DateTime[] timestamps;
				double[] values;
				string[] tooltips;
				DisplayDatasourceItem currDataItem;
				ConfiguredDatasource currConfiguredDatasource;
				Color currLineColor;
				string currDataName;

				currDataItem = _rcp.Datasources[i];

				currConfiguredDatasource = _db.ORManager.Get<ConfiguredDatasource>(currDataItem.ConfiguredDatasourceId);

				//If the datasource doesn't exist, don't process it
				if (currConfiguredDatasource == null)
					continue;

				currDataName = currConfiguredDatasource.Name;

				if (rawDataTables[i].ExtendedProperties.Contains("SubTypeId"))
				{
					DatasourceSubType currSubType;
					int subTypeId;

					subTypeId = (int) rawDataTables[i].ExtendedProperties["SubTypeId"];
					currSubType = _db.ORManager.Get<DatasourceSubType>(subTypeId);
					currDataName += " - " + currSubType.Name;
				}

				tableHelper = new DBTable(rawDataTables[i]);
				timestamps = tableHelper.getColAsDateTime(0);
				values = tableHelper.getCol(1);

				//Adjust the timestamps for the users time zone
				for (int j = 0; j < timestamps.Length; j++)
					timestamps[j] = _timeZone.ToLocalTime(timestamps[j]);

				//Create an array of strings that represent the tooltips
				tooltips = new string[timestamps.Length];
				for (int j = 0; j < timestamps.Length; j++)
					tooltips[j] = getDatapointTooltip(currConfiguredDatasource, timestamps[j], values[j]);

				//Determine the color to use
				if (currDataItem.Color == null)
					currLineColor = AutoLineColors[i%AutoLineColors.Length];
				else
					currLineColor = (Color) currDataItem.Color;

				bool useYAxis2;

				useYAxis2 = (rawDataTables.Length == 2 && i == 1);

				//Hide the axis labels when displaying 3 or more data sources
				if (rawDataTables.Length > 2)
					_chart.yAxis().setLabelFormat("");

				Axis axis = null;

				//If there are more than 2 data sources, display them on different axis
				if (rawDataTables.Length > 2)
				{
					axis = _chart.addAxis(Chart.Left, 0);
					axis.setLabelFormat("");
				}

				//Keep a running tab of the overall min and max values
				if (rawDataTables.Length <= 2)
				{
					if (!useYAxis2)
					{
						double? currYMin, currYMax;

						ChartMath.GetDataBounds(values, out currYMin, out currYMax);
						if (yMin == null && currYMin != null)
							yMin = currYMin;
						else if (currYMin != null)
							yMin = Math.Min(currYMin.Value, yMin.Value);
						if (yMax == null && currYMax != null)
							yMax = currYMax;
						else if (currYMax != null)
							yMax = Math.Min(currYMax.Value, yMax.Value);
					}
					else
					{
						double? currYMin, currYMax;

						ChartMath.GetDataBounds(values, out currYMin, out currYMax);
						if (y2Min == null && currYMin != null)
							y2Min = currYMin;
						else if (currYMin != null)
							y2Min = Math.Min(currYMin.Value, y2Min.Value);
						if (y2Max == null && currYMax != null)
							y2Max = currYMax;
						else if (currYMax != null)
							y2Max = Math.Min(currYMax.Value, y2Max.Value);
					}
				}

				if (currDataItem.ShowRaw)
				{
					LineLayer lineLayer;

					lineLayer = _chart.addLineLayer2();
					lineLayer.addDataSet(values, Chart.CColor(currLineColor), fixLayerNameWidth(currDataName)).setDataSymbol(
						Chart.DiamondSymbol, 4, 0xffff80);
					lineLayer.setXData(timestamps);
					lineLayer.addExtraField(tooltips); //field0
					lineLayer.setLineWidth(currDataItem.LineThickness);

					if (axis == null)
						lineLayer.setUseYAxis2(useYAxis2);
					else
						lineLayer.setUseYAxis(axis);
				}
				if (currDataItem.ShowTrendLine)
				{
					TrendLayer tl;
					tl =
						_chart.addTrendLayer(values, _chart.dashLineColor(Chart.CColor(currLineColor), Chart.DashLine),
						                     fixLayerNameWidth(currConfiguredDatasource.Name + " Trend"));
					tl.setLineWidth(currDataItem.LineThickness + 1);
					tl.setXData(timestamps);
					tl.addExtraField(new string[timestamps.Length]); //field0

					if (axis == null)
						tl.setUseYAxis2(useYAxis2);
					else
						tl.setUseYAxis(axis);
				}
				if (currDataItem.ShowLowess)
				{
					SplineLayer sl;
					sl =
						_chart.addSplineLayer(new ArrayMath(values).lowess().result(),
						                      _chart.dashLineColor(Chart.CColor(currLineColor), Chart.DashLine),
						                      fixLayerNameWidth(currDataName));
					sl.setLineWidth(currDataItem.LineThickness + 1);
					sl.setXData(timestamps);
					sl.addExtraField(new string[timestamps.Length]); //field0

					if (axis == null)
						sl.setUseYAxis2(useYAxis2);
					else
						sl.setUseYAxis(axis);
				}
			}

			optimizePlotArea(yMin, yMax, y2Min, y2Max);

			//Calculate the scale of the chart based on the plotted data,
			//this needs to be called before drawEvents, so the tool tip
			//locations for the events can be calculated.
			_chart.layout();

			drawEvents();
			drawLegend();
		}

		private static string fixLayerNameWidth(string name)
		{
			if (name.Length > LAYER_NAME_MAXLENGTH)
				return name.Substring(0, LAYER_NAME_MAXLENGTH) + "...";
			else
				return name;
		}

		public void GenerateChart(WebChartViewer viewer)
		{
			generateChart();
			addToolbar(viewer);

			viewer.Image = _chart.makeWebImage(Chart.PNG);

			imageMapAreas.Append(_chart.getHTMLImageMap("#", " ", "tooltip='{field0}'"));

			//Always render the tool bar
			viewer.ImageMap = imageMapAreas.ToString();
		}

		public void GenerateChart(HttpResponse response)
		{
			generateChart();

			response.ContentType = "image/png";
			response.BinaryWrite(_chart.makeChart2(Chart.PNG));
			response.End();
		}

		public byte[] GenerateChart()
		{
			generateChart();
			return _chart.makeChart2(Chart.PNG);
		}

		private void addTimestampLabels(DateTime start, DateTime end)
		{
			DateTime[] labelDates;

			labelDates = GetLabelDates(start, end, 5, 12);
			_chart.setAntiAlias(true, Chart.AntiAlias);

			foreach (DateTime currDate in labelDates)
			{
				_chart.xAxis().addLabel(Chart.CTime(currDate), currDate.ToString("d"));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		///		An image map that can be added to the main image map
		///		so that the toolbar buttons work.
		/// </returns>
		private void addToolbar(WebChartViewer viewer)
		{
			StringBuilder imageMapText;
			TextBox currIcon;
			int y;

			imageMapText = new StringBuilder();

			_chart.setSearchPath(_toolbarImagePath);

			//Start 5 pixels from the top of the chart, or the title
			y = _headerHeight + TOP_MARGIN + 5;

			currIcon = _chart.addText(5, y, "<*img=Refresh.jpg*>");
			imageMapText.Append("<area " + currIcon.getImageCoor());
			imageMapText.Append(" href=\"" + viewer.GetPostBackURL(TOOLBAR_KEY_REFRESH) + "\"");
			imageMapText.Append(" title=\"Refresh\" />");

			y += 35;
			currIcon = _chart.addText(8, y, "<*img=ZoomIn.gif*>");
			imageMapText.Append("<area " + currIcon.getImageCoor());
			imageMapText.Append(" href=\"" + viewer.GetPostBackURL(TOOLBAR_KEY_ZOOM_IN) + "\"");
			imageMapText.Append(" title=\"Zoom In\" />");

			y += 25;
			currIcon = _chart.addText(8, y, "<*img=ZoomOut.gif*>");
			imageMapText.Append("<area " + currIcon.getImageCoor());
			imageMapText.Append(" href=\"" + viewer.GetPostBackURL(TOOLBAR_KEY_ZOOM_OUT) + "\"");
			imageMapText.Append(" title=\"Zoom Out\" />");

			y += 25;
			currIcon = _chart.addText(8, y, "<*img=ScrollLeft.gif*>");
			imageMapText.Append("<area " + currIcon.getImageCoor());
			imageMapText.Append(" href=\"" + viewer.GetPostBackURL(TOOLBAR_KEY_SCROLL_LEFT) + "\"");
			imageMapText.Append(" title=\"Scroll Left\" />");

			y += 25;
			currIcon = _chart.addText(8, y, "<*img=ScrollRight.gif*>");
			imageMapText.Append("<area " + currIcon.getImageCoor());
			imageMapText.Append(" href=\"" + viewer.GetPostBackURL(TOOLBAR_KEY_SCROLL_RIGHT) + "\"");
			imageMapText.Append(" title=\"Scroll Right\" />");

			y += 25;
			currIcon = _chart.addText(8, y, "<*img=Save.gif*>");
			imageMapText.Append("<area " + currIcon.getImageCoor());
			imageMapText.Append(" href=\"" + viewer.GetPostBackURL(TOOLBAR_KEY_SAVE) + "\"");
			imageMapText.Append(" title=\"Save\" />");

			imageMapAreas.Append(imageMapText.ToString());
		}

		private static void finalizeChart()
		{
		}

		private void drawMessage(string msg)
		{
			double fontSize = _rcp.ChartSize == RankChartParameters.ChartSizes.Small ? 8.0 : 12.0;
			_chart.addText(10 + LEFT_MARGIN, 50, msg, "bold", fontSize);
		}

		private static string getDatapointTooltip(ConfiguredDatasource cd, DateTime timestamp, double value)
		{
			return
				cd.DisplayNameWithUrl + ",<br />" + cd.DatasourceType.Description + ",<br />Timestamp: " + timestamp +
				",<br />Value: " + value;
		}

		private void drawLegend()
		{
			LegendBox legend;

			legend = _chart.addLegend(10, ChartHeight - BottomMargin + 13, false);
			legend.setFontSize(8);
			legend.setCols(LegendCols);
			legend.setKeySpacing(5);
			legend.setSize(ChartWidth - 20, LegendHeight);

			//Set the background and border of the legend box to transparent
			legend.setBackground(Chart.Transparent, Chart.Transparent);
		}

		private void preSetup()
		{
			_chart.setRoundedFrame();

			if (_title != null && _title.Length > 0)
			{
				TextBox titleTextBox;

				titleTextBox = _chart.addTitle(_title);
				if (_rcp.ChartSize == RankChartParameters.ChartSizes.Small) titleTextBox.setFontSize(10.0);
				titleTextBox.setBackground(0xdddddd, 0, Chart.glassEffect());

				_headerHeight = titleTextBox.getHeight();
			}

			//Add the ranktrend website
			if (_rcp.ChartSize == RankChartParameters.ChartSizes.Small)
				_chart.addText(ChartWidth - RIGHT_MARGIN - 143, _headerHeight + 3, "http://www.RankTrend.com", "Arial", 8,
				               Chart.CColor(Color.DarkGray));
			else
				_chart.addText(ChartWidth - RIGHT_MARGIN - 173, _headerHeight + 3, "http://www.RankTrend.com", "Arial Bold", 10,
				               Chart.CColor(Color.DarkGray));

			//This needs to be done at the right time or it won't show up
			drawLegend();
		}

		private void optimizePlotArea(double? yMin, double? yMax, double? y2Min, double? y2Max)
		{
			int maxYLength = 3;
			int maxY2Length = 3;
			Rectangle plotRect;
			const int CHARACTER_WIDTH = 7;
			int tempLen;

			if (yMin != null && (tempLen = yMin.ToString().Length) > maxYLength)
				maxYLength = tempLen;
			if (yMax != null && (tempLen = yMax.ToString().Length) > maxYLength)
				maxYLength = tempLen;

			if (y2Min != null && (tempLen = y2Min.ToString().Length) > maxY2Length)
				maxY2Length = tempLen;
			if (y2Max != null && (tempLen = y2Max.ToString().Length) > maxY2Length)
				maxY2Length = tempLen;

			plotRect = new Rectangle();
			plotRect.X = LEFT_MARGIN + (CHARACTER_WIDTH*maxYLength);
			plotRect.Y = TOP_MARGIN + _headerHeight;
			plotRect.Width = ChartWidth - plotRect.X - (RIGHT_MARGIN + (CHARACTER_WIDTH*maxY2Length));
			plotRect.Height = ChartHeight - plotRect.Y - BottomMargin;

			_chart.setPlotArea(plotRect.X, plotRect.Y, plotRect.Width, plotRect.Height, 0xffffff, -1, -1, 0xcccccc, 0xcccccc);
		}

		private DataTable[] retrieveData(DisplayDatasourceItem[] datasources, DateTime start, DateTime end)
		{
			DataTable[] rawDataTables;

			rawDataTables = new DataTable[datasources.Length];

			//Retrieve the data for each datasource
			for (int i = 0; i < datasources.Length; i++)
			{
				rawDataTables[i] =
					_db.QueryRawData(start, end, datasources[i].ConfiguredDatasourceId, datasources[i].DatasourceSubTypeId);

				if (datasources[i].DatasourceSubTypeId != null)
					rawDataTables[i].ExtendedProperties["SubTypeId"] = (int) datasources[i].DatasourceSubTypeId;
			}

			return rawDataTables;
		}

		private void drawEvents()
		{
			List<DataTable> eventDataTables;

			eventDataTables = new List<DataTable>();

			//Get the data for each event category
			foreach (DisplayEventCategoryItem currCategory in _rcp.EventCategories)
			{
				DataTable currDataTable;

				currDataTable = _db.GetEventsByCategory(_start, _end, currCategory.EventCategoryId);
				eventDataTables.Add(currDataTable);
			}

			//Now process each row and add the corresponding event
			foreach (DataTable eventTable in eventDataTables)
			{
				foreach (DataRow currRow in eventTable.Rows)
				{
					string name;
					DateTime start;
					DateTime? end;
					string categoryName;
					string url;
					Color color;
					string eventLink;

					name = (string) currRow["Name"];
					start = (DateTime) currRow["StartTime"];
					if (currRow["EndTime"] == DBNull.Value)
						end = null;
					else
						end = (DateTime) currRow["EndTime"];
					if (currRow["CategoryName"] == DBNull.Value)
						categoryName = null;
					else
						categoryName = (string) currRow["CategoryName"];
					url = (string) currRow["Url"];
					if (currRow["Color"] == DBNull.Value)
						color = Color.Transparent;
					else
						color = Color.FromArgb((int) currRow["Color"]);
					if (currRow["EventLink"] == DBNull.Value)
						eventLink = null;
					else
						eventLink = (string) currRow["EventLink"];

					plotEvent(start, end, name, categoryName, url, color, eventLink);
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="label"></param>
		/// <param name="categoryName"></param>
		/// <param name="url"></param>
		/// <param name="color"></param>
		/// <param name="eventLink"></param>
		/// <returns>
		///		An area that should be appende to the image map.
		/// </returns>
		private void plotEvent(DateTime start, DateTime? end, string label, string categoryName, string url, Color color,
		                       string eventLink)
		{
			Mark startMark;
			string hoverText;
			string timeRangeString;
			string displayLabel;

			//Adjust the start time for the users time zone
			start = _timeZone.ToLocalTime(start);

			if (label.Length > EVENT_LABEL_LENGTH)
				displayLabel = label.Substring(0, EVENT_LABEL_LENGTH) + "...";
			else
				displayLabel = label;

			startMark = _chart.xAxis().addMark(Chart.CTime(start), Chart.CColor(color), displayLabel);
			startMark.setAlignment(Chart.Left);
			startMark.setFontAngle(90);
			startMark.setLineWidth(2);

			//Set up the tooltip label
			timeRangeString = start.ToString();
			if (end != null) timeRangeString += " - " + end;
			hoverText = string.Format("{0} ({1})&#013;{2}", label, categoryName, timeRangeString);
			if (eventLink != null && eventLink.Length > 0)
				hoverText += "&#013;" + eventLink;

			imageMapAreas.Append(getZoneImageMapArea(start, hoverText, eventLink));

			if (end != null)
			{
				Mark endMark;

				end = _timeZone.ToLocalTime((DateTime) end);

				startMark.setLineWidth(1);
				endMark = _chart.xAxis().addMark(Chart.CTime((DateTime) end), Chart.CColor(color));
				endMark.setLineWidth(1);

				_chart.xAxis().addZone(Chart.CTime(start), Chart.CTime((DateTime) end), Chart.CColor(color));
			}
		}

		private string getZoneImageMapArea(DateTime timestamp, string hoverText, string navigateUrl)
		{
			int markPos;
			StringBuilder zoneMap;

			markPos = _chart.getXCoor(Chart.CTime(timestamp));
			zoneMap = new StringBuilder();

			zoneMap.Append("<area shape='rect'");
			zoneMap.AppendFormat(" coords='{0},{1},{2},{3}'", markPos - 5, TOP_MARGIN, markPos + 5, ChartHeight - TOP_MARGIN);
			zoneMap.AppendFormat(" title='{0}'", hoverText);
			if (navigateUrl != null && navigateUrl.Length > 0)
			{
				zoneMap.AppendFormat(" href='{0}'", navigateUrl);
				zoneMap.Append(" target='_blank'"); //Open the link in a new window
			}
			zoneMap.Append(">");

			return zoneMap.ToString();
		}

		/// <summary>
		///		Calculates an optimal distribution for dates that will be
		///		used as labels on a chart.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="minLabelCount"></param>
		/// <param name="maxLabelCount"></param>
		/// <returns></returns>
		public static DateTime[] GetLabelDates(DateTime start, DateTime end, int minLabelCount, int maxLabelCount)
		{
			double lowestRemainder = 0;
			int highestLabelCount = minLabelCount;
			List<DateTime> outDates;
			int numberOfDays = (int) (end.Date.AddDays(1).Subtract(start.Date)).TotalDays;

			int segmentDays;

			if (numberOfDays <= minLabelCount)
			{
				//Just display 1 label per day
				highestLabelCount = numberOfDays;
			}
			else
			{
				for (int i = minLabelCount; i <= maxLabelCount; i++)
				{
					double currRemainder;

					currRemainder = numberOfDays%i;

					//First loop, set the remainder
					if (i == minLabelCount)
					{
						highestLabelCount = minLabelCount;
						lowestRemainder = currRemainder;
					}
					else if (currRemainder < lowestRemainder || currRemainder == 0)
					{
						lowestRemainder = currRemainder;
						highestLabelCount = i;
					}
				}
			}

			outDates = new List<DateTime>();
			segmentDays = (int) Math.Floor((double) numberOfDays/(double) highestLabelCount);

			//Now we know the largest divisible number
			for (int i = 0; i <= highestLabelCount; i++)
			{
				DateTime currDate;

				currDate = start.Date.AddDays(segmentDays*i);

				if (currDate <= end.AddDays(1).Date)
					outDates.Add(currDate);
			}

			return outDates.ToArray();
		}
	}
}