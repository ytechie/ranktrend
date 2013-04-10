using System;
using System.Collections.Generic;
using System.Text;
using Rss;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;
using YTech.Db;
using NHibernate;
using NHibernate.Expression;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Rt.Framework.Applications.RssFeedSubscriptions;

namespace Rt.Framework.Services.RssEventEngine
{
	public class RssEventEngine : RtEngineBase
	{
		public const int SERVICEID = 4;
		public const int ERROR_THRESHOLD = 10;

		public RssEventEngine(Database db) : base(db, false)
		{
			ReloadConfiguration();
		}

		public override int ServiceId { get { return SERVICEID; } }

		public override void Stop()
		{
			base.Stop();
			RssFeeds.ClearCache();
		}

		protected override void RunPreCycle()
		{
		}

		protected override void RunCycle()
		{
			try
			{
				int? rssSubscriptionId = _db.GetNextRssSubscriptionId();

				while (rssSubscriptionId != null)
				{
					EventRssSubscription feedSubscription = _db.ORManager.Get<EventRssSubscription>(rssSubscriptionId);
					ProcessRssSubscription(feedSubscription);
					rssSubscriptionId = _db.GetNextRssSubscriptionId();
				}

				_log.DebugFormat("{0}: No more event RSS subscriptions to process.", _name);
			}
			catch (SqlException ex)
			{
				if (Regex.IsMatch(ex.Message, "An error has occurred while establishing a connection to the server.*"))
					_log.WarnFormat("{0}:  Could not establish connection to database during cycle.", _name);
				else
					throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="feedSubscription"></param>
		public void ProcessRssSubscription(EventRssSubscription feedSubscription)
		{
			try
			{
				_log.DebugFormat("{0}: Processing feed Id {1} ({2}).", _name, feedSubscription.Id, feedSubscription.RssUrl);
				RssFeeds.ProcessFeed(feedSubscription, _db);
				feedSubscription.ErrorCount = 0;
				_db.ORManager.SaveOrUpdate(feedSubscription);
			}
			catch (Exception ex)
			{
				if (feedSubscription != null)
				{
					_db.ORManager.Refresh(feedSubscription);

					feedSubscription.ErrorCount += 1;
					_db.ORManager.SaveOrUpdate(feedSubscription);

					_log.Error(string.Format("{0}: Error processing feed {1} ({2})", _name, feedSubscription.Id, feedSubscription.RssUrl), ex);

					if (feedSubscription.ErrorCount >= ERROR_THRESHOLD)
						TextMessagesInterface.SendMessage(_db, feedSubscription.Url.UserId, string.Format("RankTrend has tried unsuccessfully {1} or more times to read the RSS feed that you specified in your RSS Subscription \"{0}\".  This could be for a number of reasons such as the URL you specified does not point to an RSS feed or the RSS feed is not available.  Please verify the URL that you specified for this feed.  If you feel this is in error, please let us know.", feedSubscription.Name, ERROR_THRESHOLD));
				}
				else
					_log.Error(ex);
			}
		}
	}
}
