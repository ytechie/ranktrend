using System;

namespace Rt.Framework.Components
{
	[Serializable]
	public class SearchEngine
	{
		private int _id;
		private string _name;

		/// <summary>
		///		The database ID of the search engine.
		/// </summary>
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		A short name that identifies the search engine.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}