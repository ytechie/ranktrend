namespace Rt.Framework.Components.Leads
{
	public class LeadType
	{
		private string _description;
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}
}