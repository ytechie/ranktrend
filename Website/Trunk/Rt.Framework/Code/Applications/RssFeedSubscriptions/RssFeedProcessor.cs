using System;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using log4net;
using Rss;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;

namespace Rt.Framework.Applications.RssFeedSubscriptions
{
	internal class RssFeedProcessor
	{
		private const int EVENT_TITLE_LENGTH = 50;
		private const int FEED_EXPIRATION_MINUTES = 30;

		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private Database _db;
		private int _expiredFailureCount = 0;
		private Timer _expiredTimer;
		private string _feedUrl;
		private bool _isRead = false;
		private RssFeed _rssFeed;

		public RssFeedProcessor(string feedUrl, Database db)
		{
			_feedUrl = feedUrl;
			_db = db;
			_expiredTimer = new Timer(ExpiredCallback);
		}

		public bool IsRead
		{
			get { return _rssFeed != null && _isRead; }
		}

		public string FeedUrl
		{
			get { return _feedUrl; }
		}

		public void Read()
		{
			_log.DebugFormat("Reading feed {0}.", _feedUrl);
			_rssFeed = RssFeed.Read(_feedUrl);
			if (_rssFeed.Exceptions.Count > 0 && _rssFeed.Exceptions.LastException != null)
				throw _rssFeed.Exceptions.LastException;
			_isRead = true;
			setExpireTimer();
		}

		public void AddEventsTo(EventCategory eventCategory)
		{
			if (!_isRead)
			{
				_log.DebugFormat("Feed {0} not read or expired.", _feedUrl);
				Read();
			}

			RssFeed rssFeed = _rssFeed;

			if (rssFeed != null && rssFeed.Channels.Count > 0)
			{
				foreach (RssItem rssItem in rssFeed.Channels[0].Items)
				{
					_db.InsertRssEvent(eventCategory.Id,
					                   rssItem.Title.Length > EVENT_TITLE_LENGTH
					                   	? rssItem.Title.Substring(0, EVENT_TITLE_LENGTH)
					                   	: rssItem.Title,
					                   getEventDescription(rssItem),
					                   rssItem.PubDate.ToUniversalTime(),
					                   eventCategory.Url.Id,
					                   rssItem.Link.ToString(),
					                   getMd5Hash(
					                   	string.Format("{0}{1}",
					                   	              rssItem.PubDate.ToUniversalTime().ToString("o", new CultureInfo("en-US")),
					                   	              rssItem.Title)));
				}
			}
		}

		private string getEventDescription(RssItem rssItem)
		{
			return string.Format("RSS Event: {0}", rssItem.Title);
		}

		private void setExpireTimer()
		{
			_log.DebugFormat("Setting feed {0} to expire in {1} minutes.", _feedUrl, FEED_EXPIRATION_MINUTES);
			_expiredTimer.Change(TimeSpan.FromMinutes(FEED_EXPIRATION_MINUTES), TimeSpan.FromMilliseconds(-1));
		}

		private void ExpiredCallback(object state)
		{
			try
			{
				_log.DebugFormat("Feed {0} has expired.  Marking it as expired.", _feedUrl);
				_isRead = false;
				_expiredFailureCount = 0;
			}
			catch (Exception ex)
			{
				_expiredFailureCount++;

				_log.Debug(string.Format("An error has occurred expiring the RSS feed {0}.", _feedUrl), ex);

				if (_expiredFailureCount >= 5 && _expiredFailureCount%5 == 0)
					_log.ErrorFormat("{0} successive errors have occurred during the feed expired cycle for feed {1}.",
					                 _expiredFailureCount, _feedUrl);
			}
			finally
			{
				setExpireTimer();
			}
		}

		// Hash an input string and return the hash as
		// a 32 character hexadecimal string.
		private static string getMd5Hash(string input)
		{
			// Create a new instance of the MD5CryptoServiceProvider object.
			MD5 md5Hasher = MD5.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
	}
}