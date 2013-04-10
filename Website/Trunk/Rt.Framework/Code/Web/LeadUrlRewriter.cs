
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web;
using Rt.Framework.Db.SqlServer;

namespace Rt.Framework.Web
{
	/// <summary>
	///		This class is used to dynamically rewrite URL's for the purpose
	///		of tracking ads and other types of leads.
	/// </summary>
	/// <remarks>
	///		To set up a new lead tracking page, add the appropriate row to
	///		the rt_LeadSources table. The "RedirectPageName" column is the name
	///		of the page under the rewrite folder that will be redirected and
	///		tracked. The "RedirectUrl" will be the destination to redirect the
	///		user to.
	/// </remarks>
	public class LeadUrlRewriter
	{
		private Dictionary<string, string> _rewriteList;
		private Database _db;

		private const string REWRITE_FOLDER = "/a/";

		/// <summary>
		///		The name of the cookie that stores the string key of
		///		the lead that is being used.
		/// </summary>
		public const string COOKIE_LEAD_KEY = "lk";

		public LeadUrlRewriter(Database dbConn)
		{
			_db = dbConn;

			_rewriteList = loadKeyMap(_db);
		}

		public LeadUrlRewriter(Dictionary<string, string> rewriteList)
		{
			_rewriteList = rewriteList;
		}

		/// <summary>
		///		Gets a copy of the rewrite list.
		/// </summary>
		public Dictionary<string, string> RewriteList
		{
			get
			{
				lock (_rewriteList)
				{
					return new Dictionary<string, string>(_rewriteList);
				}
			}
		}

		/// <summary>
		///		This should be called at the beginning of every request to
		///		determine if it should be redirected and tracked.
		/// </summary>
		public void BeginRequest()
		{
			HttpRequest request;
			string matchUrl;

			request = HttpContext.Current.Request;
			//This doesn't include the parameters passed to the page
			matchUrl = request.Url.AbsolutePath;

			lock (_rewriteList)
			{
				foreach (string key in _rewriteList.Keys)
					if (matchUrl.EndsWith(string.Format("{0}{1}.aspx", REWRITE_FOLDER, key)))
						redirect(request.ApplicationPath, key);
			}
		}

		private void redirect(string applicationPath, string key)
		{
			string redirectUrl;
			HttpCookie cookie;

			//Asynchronously record a lead hit to the database
			ThreadPool.QueueUserWorkItem(delegate { _db.Leads_IncrementLeadHit(key); });
			
			//If it's the root, we want to avoid a double slash
			if (applicationPath.Length == 1)
				applicationPath = "";

			lock (_rewriteList)
			{
				redirectUrl = _rewriteList[key].Replace("~", applicationPath);
			}

			//Create a cookie that we can check when users sign up
			cookie = new HttpCookie(COOKIE_LEAD_KEY, key);
			cookie.Expires = DateTime.UtcNow.AddDays(7);

			HttpContext.Current.Response.Cookies.Add(cookie);
			HttpContext.Current.Response.Redirect(redirectUrl);
		}

		private static Dictionary<string, string> loadKeyMap(Database db)
		{
			DataTable dt;
			Dictionary<string, string> keyMap;

			dt = db.Leads_LoadKeyMap();
			keyMap = new Dictionary<string, string>();

			foreach(DataRow row in dt.Rows)
				keyMap.Add((string)row["RedirectPageName"], (string)row["RedirectUrl"]);

			return keyMap;
		}

		public void ChangeKey(string oldKey, string newKey)
		{
			lock (_rewriteList)
			{
				string rewritePath;

				rewritePath = _rewriteList[oldKey];

				DeleteKey(oldKey);
				Add(newKey, rewritePath);
			}
		}

		public void DeleteKey(string key)
		{
			lock (_rewriteList)
			{
				_rewriteList.Remove(key);
			}
		}

		public void Add(string key, string rewritePath)
		{
			lock (_rewriteList)
			{
				_rewriteList.Add(key, rewritePath);
			}
		}

		public void ChangeRewritePath(string key, string newPath)
		{
			lock (_rewriteList)
			{
				_rewriteList[key] = newPath;
			}
		}
	}
}
