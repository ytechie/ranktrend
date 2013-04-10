using Nle.Client.RssFeeds;
using Rss;

/// <summary>
/// Summary description for RtRssSource
/// </summary>
public class RtRssSource : IRssFeedSource
{
	/// <summary>
	///		The feed signature for the RankTrend Community RSS source
	/// </summary>
	public const string FEED_SIGNATURE = "http://feeds.feedburner.com/ranktrend";

	#region IRssFeedSource Members

	public string GetCacheKey()
	{
		return "RtRssSourceCacheKey";
	}

	public RssFeed ReadFeed()
	{
		RssFeed rssFeed;
		rssFeed = RssFeed.Read(FEED_SIGNATURE);
		return rssFeed;
	}

	#endregion
}