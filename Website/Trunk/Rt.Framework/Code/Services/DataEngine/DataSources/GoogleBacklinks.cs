using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(6)]
	public class GoogleBacklinks : IDataSource
	{
		/// <summary>
		///		The URL to use to get the number of backlinks
		///		for a particular URL.
		/// </summary>
		/// <remarks>
		///		Parameters:
		///		{0} = The URL encoded version of the URL to check.
		/// </remarks>
		private const string BACKLINK_CHECK_URL = "http://www.google.com/search?q=link%3A{0}&num=1";

		/// <summary>
		///		The regex to determine the number of backlinks.
		/// </summary>
		private const string REGEX_LINK_COUNT = @"Results <b>\d+</b> - <b>\d+</b> of [about]{0-1}| <b>(\d+)</b> linking to";

		/// <summary>
		///		The string to search for to determine if there are no backlinks.
		/// </summary>
		private const string NO_RESULTS_MESSAGE = "did not match any documents";

		private bool _requestMade;
		private string _url;
		private Dictionary<int, object> _parameters;
		private int? _backlinkCount;

		public static string GetSearchUrl(string url)
		{
			return string.Format(BACKLINK_CHECK_URL, HttpUtility.UrlEncode(url));
		}

		public static int? GetBacklinkCount(string backlinkResultPage)
		{
			Regex rx;
			MatchCollection matches;

			if (backlinkResultPage == null)
				return null;

			if (backlinkResultPage.Contains(NO_RESULTS_MESSAGE))
				return 0;

			rx = new Regex(REGEX_LINK_COUNT);
			matches = rx.Matches(backlinkResultPage);
			if(matches.Count != 1 || matches[0].Groups.Count != 2)
				return null;

			return int.Parse(matches[0].Groups[1].Value);
		}

		#region IDataSource Members

		public SerializableWebRequest GetNextRequest()
		{
			SerializableWebRequest req;

			if (_requestMade)
				return null;

			req = new SerializableWebRequest();
			req.Url = GetSearchUrl(_url);

			_requestMade = true;

			return req;
		}

		public void SetResponse(SerializableWebResponse response)
		{
			_backlinkCount = GetBacklinkCount(response.Content);
		}

		public Rt.Framework.Components.RawDataValue[] Values
		{
			get
			{
				RawDataValue[] values;

				values = new RawDataValue[1];
				values[0] = new RawDataValue();
				values[0].Timestamp = DateTime.UtcNow; //Todo: get this from the database?
				values[0].FloatValue = _backlinkCount;

				if (_backlinkCount == null)
					values[0].Success = false;
				else
					values[0].Success = true;

				return values;
			}
		}

		public Dictionary<int, object> Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		#endregion
	}
}
