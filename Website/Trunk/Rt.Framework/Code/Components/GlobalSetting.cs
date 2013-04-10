using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	/// <summary>
	/// Summary description for GlobalSetting.
	/// </summary>
	public class GlobalSetting
	{
		private string _desc;
		private int _id;
		private int? _intVal;
		private string _txtVal;

		public GlobalSetting(int id)
		{
			_id = id;
		}

		public int Id
		{
			get { return _id; }
		}

		[FieldMapping("Description")]
		public string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}

		[FieldMapping("IntValue")]
		public int? IntValue
		{
			get { return _intVal; }
			set { _intVal = value; }
		}

		[FieldMapping("TextValue")]
		public string TextValue
		{
			get { return _txtVal; }
			set { _txtVal = value; }
		}
	}
}