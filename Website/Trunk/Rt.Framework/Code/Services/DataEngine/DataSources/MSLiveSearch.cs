using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(7)]
	public class MSLiveSearch : IDataSource
	{
		private const string SEARCH_URL = "http://search.live.com/results.aspx?q={0}&format=rss&count=100";

		private const string REGEX_SEARCH_URLS = "<item>.+?<link>(.+?)</link>.+?</item>";

		private bool _requestMade;
		private string _searchPhrase;
		private string _url;
		private int? _position;
		private Dictionary<int, object> _parameters;

		public static int? GetSearchPosition(string rssSearchResults, string url)
		{
			Regex rx;
			MatchCollection matches;

			rx = new Regex(REGEX_SEARCH_URLS);
			matches = rx.Matches(rssSearchResults);

			for(int i = 0; i < matches.Count; i++)
			{
				if (matches[i].Groups[1].Value.StartsWith(url))
					return i + 1;
			}

			return 0;
		}

		private void readParameters()
		{
			_searchPhrase = (string)_parameters[1];
		}

		#region IDataSource Members

		public SerializableWebRequest GetNextRequest()
		{
			SerializableWebRequest req;

			if (_requestMade)
				return null;

			readParameters();

			req = new SerializableWebRequest();
			req.Url = string.Format(SEARCH_URL, _searchPhrase);

			_requestMade = true;

			return req;
		}

		public void SetResponse(SerializableWebResponse response)
		{
			_position = GetSearchPosition(response.Content, _url);
		}

		public RawDataValue[] Values
		{
			get
			{
				RawDataValue[] values;

				values = new RawDataValue[1];
				values[0] = new RawDataValue();
				values[0].FloatValue = _position.Value;
				values[0].Success = true;
				values[0].Timestamp = DateTime.UtcNow;

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
