namespace Rt.Framework.Components
{
	public class CustomReportComponent
	{
		private CustomReportComponentType _componentType;
		private ConfiguredDatasource _configuredDatasource;
		private CustomReport _customReport;
		private DatasourceSubType _datasourceSubType;
		private int? _id;
		private SavedReport _savedReport;
		private UrlClass _url;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public CustomReportComponentType ComponentType
		{
			get { return _componentType; }
			set { _componentType = value; }
		}

		public ConfiguredDatasource ConfiguredDatasource
		{
			get { return _configuredDatasource; }
			set { _configuredDatasource = value; }
		}

		public DatasourceSubType DatasourceSubType
		{
			get { return _datasourceSubType; }
			set { _datasourceSubType = value; }
		}

		public SavedReport SavedReport
		{
			get { return _savedReport; }
			set { _savedReport = value; }
		}

		public UrlClass Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public CustomReport CustomReport
		{
			get { return _customReport; }
			set { _customReport = value; }
		}
	}
}