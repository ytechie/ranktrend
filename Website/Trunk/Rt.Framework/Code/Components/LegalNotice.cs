using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	public class LegalNotice
	{
		private string _description;
		private int? _id;

		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[FieldMapping("Description")]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}
}