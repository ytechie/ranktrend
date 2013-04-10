using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	public class UserInformation
	{
		private string _firstName;
		private string _lastName;
		private string _leadKey;
		private object _userId;

		[FieldMapping("UserId")]
		public object UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		[FieldMapping("FirstName")]
		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		[FieldMapping("LastName")]
		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		public string LeadKey
		{
			get { return _leadKey; }
			set { _leadKey = value; }
		}

		public string FullName
		{
			get { return string.Format("{0} {1}", FirstName, LastName); }
		}
	}
}