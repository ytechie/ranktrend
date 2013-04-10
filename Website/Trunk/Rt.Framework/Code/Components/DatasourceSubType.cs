using System;

namespace Rt.Framework.Components
{
	[Serializable]
	public class DatasourceSubType
	{
		private DatasourceType _datasourceType;
		private int _datasourceTypeId;
		private int? _id;
		private string _name;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int DatasourceTypeId
		{
			get { return _datasourceTypeId; }
			set { _datasourceTypeId = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public DatasourceType DatasourceType
		{
			get { return _datasourceType; }
			set { _datasourceType = value; }
		}
	}
}