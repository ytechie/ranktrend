using System;

namespace Rt.Framework.Components
{
	public class SavedReport
	{
		private DateTime _created;
		private string _description = string.Empty;
		private int? _id;
		private DateTime? _lastSaved;
		private string _name;
		private bool _publicViewable = false;
		private int _reportTypeId;
		private Guid _userId;
		private string _xmlData;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int ReportTypeId
		{
			get { return _reportTypeId; }
			set { _reportTypeId = value; }
		}

		public string XmlData
		{
			get { return _xmlData; }
			set { _xmlData = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public bool PublicViewable
		{
			get { return _publicViewable; }
			set { _publicViewable = value; }
		}

		public Guid UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		public DateTime Created
		{
			get { return _created; }
			set { _created = value; }
		}

		public DateTime? LastSaved
		{
			get { return _lastSaved; }
			set { _lastSaved = value; }
		}
	}
}