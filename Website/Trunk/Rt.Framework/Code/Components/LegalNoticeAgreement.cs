using System;
using System.Web.Security;
using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	/// <summary>
	/// Summary description for LegalNoticeAgreement.
	/// </summary>
	public class LegalNoticeAgreement
	{
		private bool _agree;
		private int? _id;
		private int _legalNoticeVersionId;
		private DateTime _timestamp;
		private object _userId;

		public LegalNoticeAgreement()
		{
		}

		public LegalNoticeAgreement(LegalNoticeVersion legalNoticeVersion, MembershipUser user, bool agree)
		{
			LegalNoticeVersionId = (int) legalNoticeVersion.Id;
			UserId = user.ProviderUserKey;
			Agree = agree;
		}

		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[FieldMapping("LegalNoticeId")]
		public int LegalNoticeVersionId
		{
			get { return _legalNoticeVersionId; }
			set { _legalNoticeVersionId = value; }
		}

		[FieldMapping("UserId")]
		public object UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		[FieldMapping("Agree")]
		public bool Agree
		{
			get { return _agree; }
			set { _agree = value; }
		}

		[FieldMapping("Timestamp")]
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}
	}
}