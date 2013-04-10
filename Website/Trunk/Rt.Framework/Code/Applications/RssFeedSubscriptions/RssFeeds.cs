using System.Collections.Generic;
using System.Reflection;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;

namespace Rt.Framework.Applications.RssFeedSubscriptions
{
	public static class RssFeeds
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		///		A cached list of RSS feeds.  The <see cref="RssFeedProcessor"/> handles the expiring
		///		and reloading of the individual feeds.
		/// </summary>
		private static Dictionary<string, RssFeedProcessor> _cachedFeeds = new Dictionary<string, RssFeedProcessor>();

		/// <summary>
		///		Removes the specified RSS feed from the cache.
		/// </summary>
		/// <param name="url">
		///		The RSS feed to remove from the cache.
		/// </param>
		public static void ClearCache(string url)
		{
			if (_cachedFeeds.ContainsKey(url.ToLower()))
				_cachedFeeds.Remove(url.ToLower());
		}

		/// <summary>
		///		Clears all the cached RSS feeds.
		/// </summary>
		public static void ClearCache()
		{
			_cachedFeeds.Clear();
		}

		/// <summary>
		///		Reads the RSS feed specified in the <see cref="EventRssSubscription"/> and
		///		inserts the new RSS items into the database.
		/// </summary>
		/// <param name="eventRssSubscription">
		///		The <see cref="EventRssSubscription"/> whose RSS feed should be read and
		///		processed.
		/// </param>
		/// <param name="db">
		///		The connection to the database where the new RSS itmes
		///		should be recorded.
		/// </param>
		public static void ProcessFeed(EventRssSubscription eventRssSubscription, Database db)
		{
			ProcessFeed(eventRssSubscription.RssUrl, db, eventRssSubscription.EventCategory);
		}

		/// <summary>
		///		Reads the RSS feed specified and inserts the new RSS items into the
		///		database.
		/// </summary>
		/// <param name="url">
		///		The URL of the RSS feed.
		/// </param>
		/// <param name="db">
		///		The connection to the database where the new RSS items
		///		should be recorded.
		/// </param>
		/// <param name="eventCategory">
		///		The <see cref="EventCategory"/> that the RSS events belong.
		/// </param>
		/// <remarks>
		///		Feeds are cached to avoid reading the same RSS feed too often.
		/// </remarks>
		public static void ProcessFeed(string url, Database db, EventCategory eventCategory)
		{
			RssFeedProcessor feedProcessor;

			if (_cachedFeeds.ContainsKey(url.ToLower()))
			{
				_log.DebugFormat("Found RSS feed {0} in cached list.", url);
				feedProcessor = _cachedFeeds[url.ToLower()];
			}
			else
			{
				_log.DebugFormat("RSS feed {0} not found in cached list.  Creating new feed and reading.", url);
				feedProcessor = new RssFeedProcessor(url, db);
				_cachedFeeds.Add(url, feedProcessor);
				feedProcessor.Read();
			}

			feedProcessor.AddEventsTo(eventCategory);
		}
	}
}