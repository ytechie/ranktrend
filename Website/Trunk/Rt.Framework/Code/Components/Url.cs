using System;
using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	/// <summary>
	/// 
	/// </summary>
	///	<remarks>
	///		This class is not named "URL", because that would conflict
	///		with the Url property.
	///	</remarks>
	[Serializable]
	public class UrlClass
	{
		private int _id;
		private string _url;
		private Guid _userId;

		[FieldMapping("Id")]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[FieldMapping("Url")]
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		[FieldMapping("UserId")]
		public Guid UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}
	}
}