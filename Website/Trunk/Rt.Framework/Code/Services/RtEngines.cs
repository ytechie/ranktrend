using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Rt.Framework.Db;
using EmailEngineNs = Rt.Framework.Services.EmailEngine;
using ReportEngineNs = Rt.Framework.Services.ReportEngine;
using RssEventEngineNs = Rt.Framework.Services.RssEventEngine;
using Rt.Framework.Db.SqlServer;

namespace Rt.Framework.Services
{
	/// <summary>
	///     Provides static instances of the various service engines.
	/// </summary>
	public static class RtEngines
	{
#if SANDBOX
		public const string CONFIG_DB_CONNECTION_STRING = "DbSandboxConnectionString";
#else
		public const string CONFIG_DB_CONNECTION_STRING = "main";
#endif

		private const int WEB_EMAILENGINE_SERVICEID = 2;
		private const int WEB_RSSEVENTENGINE_SERVICEID = 5;
		private const int WEB_REPORTENGINE_SERVICEID = 6;
		private const int WEB_DATAINTEGRITYENGINE_SERVICEID = 8;

		private static Database _db;
		private static EmailEngineNs.EmailEngine _emailEngine;
		private static WebEngine _webEmailEngine;
		private static ReportEngineNs.ReportEngine _reportEngine;
		private static WebEngine _webReportEngine;
		private static RssEventEngineNs.RssEventEngine _rssEventEngine;
		private static WebEngine _webRssEventEngine;

		private static Database Db
		{
			get
			{
				if (_db == null)
					_db = getDbConnection();

				return _db;
			}
		}

		/// <summary>
		///     Gets the static Email Engine instance.
		/// </summary>
		public static EmailEngineNs.EmailEngine EmailEngine
		{
			get
			{
				if (_emailEngine == null)
					_emailEngine = new EmailEngineNs.EmailEngine(getDbConnection());

				return _emailEngine;
			}
		}

		public static WebEngine WebEmailEngine
		{
			get
			{
				if (_webEmailEngine == null)
					_webEmailEngine = new WebEngine(getDbConnection(), WEB_EMAILENGINE_SERVICEID, EmailEngineNs.EmailEngine.SERVICEID);

				return _webEmailEngine;
			}
		}

		public static ReportEngineNs.ReportEngine ReportEngine
		{
			get
			{
				if (_reportEngine == null)
					_reportEngine = new ReportEngineNs.ReportEngine(getDbConnection());

				return _reportEngine;
			}
		}

		public static WebEngine WebReportEngine
		{
			get
			{
				if (_webReportEngine == null)
					_webReportEngine = new WebEngine(getDbConnection(), WEB_REPORTENGINE_SERVICEID, ReportEngineNs.ReportEngine.SERVICEID);

				return _webReportEngine;
			}
		}

		public static RssEventEngineNs.RssEventEngine RssEventEngine
		{
			get
			{
				if (_rssEventEngine == null)
					_rssEventEngine = new RssEventEngineNs.RssEventEngine(getDbConnection());

				return _rssEventEngine;
			}
		}

		public static WebEngine WebRssEventEngine
		{
			get
			{
				if (_webRssEventEngine == null)
					_webRssEventEngine = new WebEngine(getDbConnection(), WEB_RSSEVENTENGINE_SERVICEID, RssEventEngineNs.RssEventEngine.SERVICEID);

				return _webRssEventEngine;
			}
		}

		public static void ForceEngineHeartbeats()
		{
			EmailEngine.ForceHeartbeat();
			WebEmailEngine.ForceHeartbeat();
			ReportEngine.ForceHeartbeat();
			WebReportEngine.ForceHeartbeat();
			RssEventEngine.ForceHeartbeat();
			WebRssEventEngine.ForceHeartbeat();
		}

		public static RtEngineBase GetRtEngine(int serviceId)
		{
			switch (serviceId)
			{
				case EmailEngineNs.EmailEngine.SERVICEID:
					return _emailEngine;
				case WEB_EMAILENGINE_SERVICEID:
					return _webEmailEngine;
				case ReportEngineNs.ReportEngine.SERVICEID:
					return _reportEngine;
				case WEB_REPORTENGINE_SERVICEID:
					return _webReportEngine;
				case RssEventEngineNs.RssEventEngine.SERVICEID:
					return _rssEventEngine;
				case WEB_RSSEVENTENGINE_SERVICEID:
					return _webRssEventEngine;
				default:
					throw new NotSupportedException(string.Format("{0} is not a supported service Id.", serviceId));
			}
		}

		public static string GetRtEngineName(int serviceId)
		{
			Components.Service service = Db.GetService(serviceId);
			return service.Description;
		}

		/// <summary>
		///		Creates and prepares a <see cref="Database"/> object that
		///		is ready to interact with the database.
		/// </summary>
		/// <returns></returns>
		private static Database getDbConnection()
		{
			ConnectionStringSettings connStringSettings;
			string dbConnectionString;
			connStringSettings = ConfigurationManager.ConnectionStrings[CONFIG_DB_CONNECTION_STRING];
			dbConnectionString = connStringSettings.ConnectionString;
			return ConnectionFactory.GetDbConnectionFromConnectionString(dbConnectionString);
		}
	}
}
