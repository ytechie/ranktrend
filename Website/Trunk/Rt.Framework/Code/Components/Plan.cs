using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	public class Plan
	{
		private bool _adFree;
		private bool _customEvents;
		private bool _dailyEmailReports;
		private string _friendlyName;
		private bool _globalEvents;
		private int? _id;
		private bool _importExport;
		private double _monthlyPrice;
		private bool _plotRank;
		private double _rankCheckRate;
		private int _savedReports;
		private bool _unlimitedSubPageRank;
		private int _urls;
		private double _yearlyPrice;

		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[FieldMapping("FriendlyName")]
		public string FriendlyName
		{
			get { return _friendlyName; }
			set { _friendlyName = value; }
		}

		[FieldMapping("PlotRank")]
		public bool PlotRank
		{
			get { return _plotRank; }
			set { _plotRank = value; }
		}

		[FieldMapping("RankCheckRate")]
		public double RankCheckRate
		{
			get { return _rankCheckRate; }
			set { _rankCheckRate = value; }
		}

		[FieldMapping("Urls")]
		public int Urls
		{
			get { return _urls; }
			set { _urls = value; }
		}

		[FieldMapping("UnlimitedSubPageRank")]
		public bool UnlimitedSubPageRank
		{
			get { return _unlimitedSubPageRank; }
			set { _unlimitedSubPageRank = value; }
		}

		[FieldMapping("AdFree")]
		public bool AdFree
		{
			get { return _adFree; }
			set { _adFree = value; }
		}

		[FieldMapping("GlobalEvents")]
		public bool GlobalEvents
		{
			get { return _globalEvents; }
			set { _globalEvents = value; }
		}

		[FieldMapping("ImportExport")]
		public bool ImportExport
		{
			get { return _importExport; }
			set { _importExport = value; }
		}

		[FieldMapping("SavedReports")]
		public int SavedReports
		{
			get { return _savedReports; }
			set { _savedReports = value; }
		}

		[FieldMapping("CustomEvents")]
		public bool CustomEvents
		{
			get { return _customEvents; }
			set { _customEvents = value; }
		}

		[FieldMapping("DailyEmailReports")]
		public bool DailyEmailReports
		{
			get { return _dailyEmailReports; }
			set { _dailyEmailReports = value; }
		}

		public double MonthlyPrice
		{
			get { return _monthlyPrice; }
			set { _monthlyPrice = value; }
		}

		public double YearlyPrice
		{
			get { return _yearlyPrice; }
			set { _yearlyPrice = value; }
		}
	}
}