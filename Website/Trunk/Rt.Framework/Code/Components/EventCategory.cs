namespace Rt.Framework.Components
{
	public class EventCategory
	{
		private int _id;
		private string _name;
		private UrlClass _url;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public UrlClass Url
		{
			get { return _url; }
			set { _url = value; }
		}
	}
}