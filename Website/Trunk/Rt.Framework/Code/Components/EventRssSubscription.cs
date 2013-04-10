using System;

namespace Rt.Framework.Components
{
	public class EventRssSubscription
	{
		private string _description;
		private int? _errorCount;
		private EventCategory _eventCategory;
		private int? _id;
		private DateTime? _lastCheck;
		private string _name;
		private string _rssUrl;
		private UrlClass _url;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public DateTime? LastCheck
		{
			get { return _lastCheck; }
			set { _lastCheck = value; }
		}

		public string RssUrl
		{
			get { return _rssUrl; }
			set { _rssUrl = value; }
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

		public EventCategory EventCategory
		{
			get { return _eventCategory; }
			set { _eventCategory = value; }
		}

		public UrlClass Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public int? ErrorCount
		{
			get { return _errorCount; }
			set { _errorCount = value; }
		}
	}
}