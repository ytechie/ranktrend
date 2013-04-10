using System;

namespace Rt.Framework.Components
{
	[Serializable]
	public class TextMessage
	{
		private bool _acknowledged;
		private int _id;
		private string _message;
		private DateTime _timestamp;
		private Guid _userId;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public bool Acknowledged
		{
			get { return _acknowledged; }
			set { _acknowledged = value; }
		}

		public Guid UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}
	}
}