using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Db;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;

namespace Rt.Website
{
	/// <summary>
	///		The global application class where the global application events occurr
	/// </summary>
	public class Global : HttpApplication
	{
#if SANDBOX
		public const string CONFIG_DB_CONNECTION_STRING = "DbSandboxConnectionString";
#else
		public const string CONFIG_DB_CONNECTION_STRING = "main";
#endif

		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly Dictionary<int, Plan> _planCache = new Dictionary<int, Plan>();
		private static string _dbConnectionString;

		public Global()
		{
			BeginRequest += Global_BeginRequest;
		}

		public static string DBConnectionString
		{
			get { return _dbConnectionString; }
		}

		/// <summary>
		///		Gets the virtual directory of the current application.
		/// </summary>
		public static string VirtualDirectory
		{
			get
			{
				if (HttpContext.Current.Request.ApplicationPath == "/")
					return "/";
				else
					return HttpContext.Current.Request.ApplicationPath + "/";
			}
		}

		/// <summary>
		///		Gets the domain of the current application.
		/// </summary>
		public static string Domain
		{
			get
			{
				if (HttpContext.Current.Request.Url.Port == -1)
					return HttpContext.Current.Request.Url.Host;
				else
					return string.Format("{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);
			}
		}

		#region Application Events

		private void Application_Start(object sender, EventArgs e)
		{
			ConnectionStringSettings connStringSettings;

			_log.Info("Application Logging Started");

			connStringSettings = ConfigurationManager.ConnectionStrings[CONFIG_DB_CONNECTION_STRING];
			_dbConnectionString = connStringSettings.ConnectionString;

			_log.DebugFormat("Loaded database connection string: '{0}'", _dbConnectionString);

			// Start Web Services
			_log.Debug("Forcing Engine Heartbeats");
			RtEngines.ForceEngineHeartbeats();

			EndRequest += Global_EndRequest;
		}

		private void Global_BeginRequest(object s, EventArgs e)
		{
			if (Context.Request.QueryString["trace"] == "true")
				Context.Trace.IsEnabled = true;
		}

		private static void Global_EndRequest(object sender, EventArgs e)
		{
		}

		private static void Application_End(object sender, EventArgs e)
		{
			//  Code that runs on application shutdown
		}

		private void Application_Error(object sender, EventArgs e)
		{
			// Code that runs when an unhandled error occurs
			Exception lastError;
			lastError = Server.GetLastError();
			_log.Fatal("An unhandled exception occurred.", lastError);
		}

		private static void Session_Start(object sender, EventArgs e)
		{
			// Code that runs when a new session is started
		}

		private static void Session_End(object sender, EventArgs e)
		{
			// Code that runs when a session ends. 
			// Note: The Session_End event is raised only when the sessionstate mode
			// is set to InProc in the Web.config file. If session mode is set to StateServer 
			// or SQLServer, the event is not raised.
		}

		#endregion

		/// <summary>
		///		Creates and prepares a <see cref="Database"/> object that
		///		is ready to interact with the database.
		/// </summary>
		/// <returns></returns>
		public static Database GetDbConnection()
		{
			return ConnectionFactory.GetDbConnectionFromConnectionString(_dbConnectionString);
		}

		public static DateTime ConvertToUserTime(DateTime utcTime)
		{
			return utcTime;
			//Win32TimeZone timeZone;

			//if (HttpContext.Current == null)
			//{
			//  _log.Warn("'ConvertToUserTime' was called, but an HttpContext is not available to load the users time zone");
			//  return utcTime;
			//}

			//timeZone = TimeZones.GetTimeZone(((ProfileCommon) HttpContext.Current.Profile).TimeZoneIndex);

			//return timeZone.ToLocalTime(utcTime);
		}

		public static TimeSpan GetUserUtcOffset()
		{
			DateTime timestamp;
			DateTime localTimestamp;

			timestamp = DateTime.UtcNow;
			localTimestamp = ConvertToUserTime(timestamp);

			return localTimestamp.Subtract(timestamp);
		}

		public static string ResolveUrl(string url)
		{
			string urlBase;

			if (HttpContext.Current == null)
			{
				_log.Warn("The current HTTP context is NULL, so a default site URL will be used.");
				urlBase = string.Format("http://www.RankTrend.com");
				return urlBase;
			}

			if (HttpContext.Current.Request.ApplicationPath == "/")
				urlBase = string.Format("http://{0}", Domain);
			else
				urlBase = string.Format("http://{0}{1}", Domain, HttpContext.Current.Request.ApplicationPath);

			return url.Replace("~", urlBase);
		}

		/// <summary>
		///		Loads the subscription plan for the currently logged on user.
		/// </summary>
		/// <remarks>
		///		This is the central method that should be used from every page
		///		to determine what capabilities the logged in user has based on
		///		their current plan level.
		/// </remarks>
		/// <returns>
		///		That plan corresponding to the subscription level of the user.  If
		///		the user doesn't have a subscription, the plan level of 0 will be
		///		returned;
		/// </returns>
		/// <example>
		///		Plan plan = Global.LoadSubscriptionPlan();
		/// </example>
		public static Plan LoadSubscriptionPlan(MembershipUser user)
		{
			int planId;
			Database db;
			Guid userId;
			Plan plan;

			plan = HttpContext.Current.Session[SessionCache.USER_PLAN] as Plan;
			if (plan != null)
			{
				_log.DebugFormat("The users plan was stored in their session, it was plan #{0}", plan.Id);
				return plan;
			}

			_log.Debug("The users plan is not in the session, we'll have to look it up");

			user = Membership.GetUser();
			userId = (Guid) user.ProviderUserKey;

			db = GetDbConnection();
			//This will return 1 if the user doesn't have a subscription
			planId = db.Plans_GetUsersPlanId(userId);

			lock (_planCache)
			{
				if (_planCache.ContainsKey(planId))
				{
					plan = _planCache[planId];
				}
				else
				{
					plan = db.ORManager.Get<Plan>(planId);
					_planCache.Add(planId, plan);
				}
			}

			//Cache the plan for the user
			HttpContext.Current.Session[SessionCache.USER_PLAN] = plan;

			return plan;
		}

		public override string GetVaryByCustomString(HttpContext context, string custom)
		{
			if (custom == "staticWhenLoggedOut")
			{
				if (context.User.Identity.IsAuthenticated)
				{
					return context.User.Identity.Name;
				}
				else
				{
					return "static";
				}
			}
			else
			{
				return base.GetVaryByCustomString(context, custom);
			}
		}

		public static void AddCommonJavaScript(Page page)
		{
			page.ClientScript.RegisterClientScriptInclude("jquery", ResolveUrl("~/Scripts/jquery.js"));
			page.ClientScript.RegisterClientScriptInclude("roundedCorners", ResolveUrl("~/Scripts/Rounded-Corners.js"));
			page.ClientScript.RegisterClientScriptInclude("common", ResolveUrl("~/Scripts/common.js"));
		}

		public static void AddTooltipJavaScript(Page page)
		{
			page.ClientScript.RegisterClientScriptInclude("iutil", ResolveUrl("~/Scripts/iutil.js"));
			page.ClientScript.RegisterClientScriptInclude("itooltips", ResolveUrl("~/Scripts/itooltip.js"));
		}
	}
}