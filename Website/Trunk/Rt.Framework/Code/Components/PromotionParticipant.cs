using System;

namespace Rt.Framework.Components
{
	public class PromotionParticipant
	{
		private int? _id;
		private Promotion _promotion;
		private DateTime _timestamp;
		private Guid? _userId = null;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Promotion Promotion
		{
			get { return _promotion; }
			set { _promotion = value; }
		}

		public Guid? UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}
	}
}