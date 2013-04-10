using System;
using System.IO;
using System.Xml.Serialization;
using Rt.Framework.CommonControls.Web;

namespace Rt.Framework.Applications.InteractiveReport
{
	[Serializable]
	public class RankChartParameters
	{
		#region ChartSizes enum

		public enum ChartSizes
		{
			Small = 0,
			Medium = 1,
			Large = 2
		}

		#region Private fields

		//Default to the last 30 days
		private ChartSizes _chartSize = ChartSizes.Medium;
		private DisplayDatasourceItem[] _datasources;
		private DateRangeSelector.DateRanges _dateRangeType = DateRangeSelector.DateRanges.Last30Days;
		private DateTime _endTime;
		private DisplayEventCategoryItem[] _eventCategories;
		private DateTime _startTime;
		private int _timezoneOffsetIndex = 20; //default to central

		#endregion

		#region Constructors

		public RankChartParameters()
		{
		}

		#endregion

		#region Properties/Accessors (Note: Remember these get serialized)

		public int TimezoneOffsetIndex
		{
			get { return _timezoneOffsetIndex; }
			set { _timezoneOffsetIndex = value; }
		}

		public DateRangeSelector.DateRanges DateRangeType
		{
			get { return _dateRangeType; }
			set { _dateRangeType = value; }
		}

		public DateTime StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		public DateTime EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		public DisplayDatasourceItem[] Datasources
		{
			get { return _datasources; }
			set { _datasources = value; }
		}

		public DisplayEventCategoryItem[] EventCategories
		{
			get { return _eventCategories; }
			set { _eventCategories = value; }
		}

		public ChartSizes ChartSize
		{
			get { return _chartSize; }
			set { _chartSize = value; }
		}

		#endregion

		#region Scrolling / Zooming

		public static void ScrollTimeRange(DateTime start, DateTime end, out DateTime newStart, out DateTime newEnd,
		                                   bool scrollLeft)
		{
			TimeSpan range;
			double offset;

			if (scrollLeft)
				offset = -.75;
			else
				offset = .75;

			range = end.Subtract(start);
			newStart = start.AddTicks((long) (range.Ticks*offset));
			newEnd = end.AddTicks((long) (range.Ticks*offset));
		}

		public static void ZoomTimeRange(DateTime start, DateTime end, out DateTime newStart, out DateTime newEnd, bool zoomIn)
		{
			TimeSpan range;
			double marginSize;

			if (zoomIn)
				marginSize = .25;
			else
				marginSize = -.5;

			range = end.Subtract(start);
			newStart = start.AddTicks((long) (range.Ticks*marginSize));
			newEnd = end.AddTicks((long) (range.Ticks*-marginSize));
		}

		#endregion

		#region Xml Serialization

		public string XmlSerialize()
		{
			XmlSerializer serializer;
			StringWriter sw;
			string xml;

			serializer = new XmlSerializer(typeof (RankChartParameters));
			using (sw = new StringWriter())
			{
				serializer.Serialize(sw, this);
				xml = sw.ToString();
			}

			return xml;
		}

		public static RankChartParameters XmlDeserialize(string xml)
		{
			XmlSerializer serializer;
			StringReader sr;
			RankChartParameters rcp;

			serializer = new XmlSerializer(typeof (RankChartParameters));
			using (sr = new StringReader(xml))
			{
				rcp = serializer.Deserialize(sr) as RankChartParameters;
			}

			return rcp;
		}

		#endregion

		#endregion
	}
}