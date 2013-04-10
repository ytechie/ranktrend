using System.Collections.Generic;
using System.Web.SessionState;
using Rt.Framework.Components;

/// <summary>
/// Summary description for CacheKeys
/// </summary>
public static class SessionCache
{
	/// <summary>
	///		Data Type: Generic IList of Site objects.
	/// </summary>
	private const string CACHE_KEY_SITE_LIST = "SiteList";

	public const string USER_PLAN = "UserPlan";

	public static IList<UrlClass> GetSiteList(HttpSessionState session)
	{
		IList<UrlClass> siteList;

		siteList = session[CACHE_KEY_SITE_LIST] as IList<UrlClass>;

		return siteList;
	}

	public static void SaveSiteList(HttpSessionState session, IList<UrlClass> sites)
	{
		session[CACHE_KEY_SITE_LIST] = sites;
	}
}