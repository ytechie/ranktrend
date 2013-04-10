using System;

namespace Rt.Framework.Api.Model
{
	public class DataPoint
	{
		public int ConfiguredDatasourceId { get; set;}
		public int? SubTypeId { get; set;}
		public double? Value { get; set; }
		public bool Fuzzy { get; set; }
		public bool Success { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
