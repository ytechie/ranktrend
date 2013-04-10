using System;

namespace Rt.Framework.Components
{
	public class DatasourceDataQueueItem
	{
		private string _data;
		private int? _id;
		private DateTime _lastAttempt;
		private long _rawDataId;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Data
		{
			get { return _data; }
			set { _data = value; }
		}

		public long RawDataId
		{
			get { return _rawDataId; }
			set { _rawDataId = value; }
		}

		/// <summary>
		///		The last time that we tried to process
		///		the data. This is used to determine when
		///		it should be tried again.
		/// </summary>
		public DateTime LastAttempt
		{
			get { return _lastAttempt; }
			set { _lastAttempt = value; }
		}
	}
}