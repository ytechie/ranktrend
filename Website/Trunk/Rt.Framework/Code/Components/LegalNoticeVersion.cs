using System;
using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	/// <summary>
	/// Summary description for LegalNoticeVersion.
	/// </summary>
	public class LegalNoticeVersion
	{
		private int? _id;
		private int _legalNoticeId;
		private string _notice;
		private DateTime _timestamp;

		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[FieldMapping("LegalNoticeId")]
		public int LegalNoticeId
		{
			get { return _legalNoticeId; }
			set { _legalNoticeId = value; }
		}

		[FieldMapping("Notice")]
		public string Notice
		{
			get { return _notice; }
			set { _notice = value; }
		}

		[FieldMapping("Timestamp")]
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}
	}
}