namespace Rt.Framework.Components
{
	public class CustomReportComponentType
	{
		private string _description;
		private int? _id;
		private bool _isHtml;
		private string _name;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public bool IsHtml
		{
			get { return _isHtml; }
			set { _isHtml = value; }
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
	}
}