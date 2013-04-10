namespace Rt.Framework.Components
{
	public class EmailFilter
	{
		private string _description;
		private string _displayName;
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string DisplayName
		{
			get { return _displayName; }
			set { _displayName = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}
}