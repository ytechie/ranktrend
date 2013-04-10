using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using log4net;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(2)]
	public class GoogleRank : IDataSource
	{
		private const string SEARCH_URL = "http://www.google.com/search?q={0}&num=100";
		private const string SEARCH_RESULT_REGEX = @"<h2\s+class=""{0,1}r""{0,1}><a\s+href=""(?:/interstitial\?url=)??(?<Url>\w+://[^\""]+?)""\s*(?:class=""{0,1}l""{0,1}){0,1}\s*(?:onmousedown=""{0,1}[^""]*""{0,1}){0,1}>(?!Image results for)|<font (?:size=""{0,1}-1""{0,1}){0,1}\s*color=""{0,1}#008000""{0,1}\s*(?:size=""{0,1}-1""{0,1}){0,1}>(?<Url>.+?)</font>";
		//private const string SEARCH_RESULT_COUNT_REGEX = @" <b>1</b> - <b>(\d+)</b> of about <b>[\d,]+</b> for";
		private const string SEARCH_RESULT_COUNT_REGEX = @"Results <b>1</b> - <b>(\d+)</b> of(?: about){0,1} <b>(?:[\d,]+)</b>";

		private string _url;
		private Dictionary<int, object> _parameters;
		private string _searchPhrase;
		private bool _requestMade;
		private int? _searchPosition;
		private bool _errorState;
		private bool _fuzzy;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public GoogleRank()
		{
			_requestMade = false;
		}

		private void readParameters()
		{
			_searchPhrase = (string)_parameters[1];
		}

		#region IDataSource Members

		public SerializableWebRequest GetNextRequest()
		{
			SerializableWebRequest request;

			if (_requestMade)
			{
				//We only want to make the request once, and then we're done.
				return null;
			}
			else
			{
				_requestMade = true;
				readParameters();
			}

			request = new SerializableWebRequest();
			request.Url = string.Format(GoogleRank.SEARCH_URL, _searchPhrase);

			return request;
		}

		public void SetResponse(SerializableWebResponse response)
		{
			Regex re;
			MatchCollection matches;
			int resultCount;

			re = new Regex(SEARCH_RESULT_REGEX);
			matches = re.Matches(response.Content);

			//Check if the page has no results
			if(response.Content.Contains("did not match any documents"))
			{
				_searchPosition = null;
				return;
			}

			//Check the number of results to make sure our regex isn't broken
			resultCount = getSearchResultCount(response.Content);

			if (matches.Count != resultCount)
			{
				_log.WarnFormat("There were {0} matches, but the page says there should be {1} results", matches.Count, resultCount);
				_log.DebugFormat("Dumping response for analysis: {0}", response.Content);

				if (matches.Count >= resultCount - 3 && matches.Count <= resultCount + 3)
				{
					_fuzzy = true;
				}
				else
				{
					_log.Debug("The result is too far off to be considered fuzzy");
					_errorState = true;
					return;
				}
			}

			for(int i = 0; i < matches.Count; i++)
			{
				if(matches[i].Groups[1].Value.StartsWith(Url, true, CultureInfo.InvariantCulture))
				{
					//we found a match

					//Remember, item 0 in the array is search result #1
					_searchPosition = i + 1;
					break;
				}
			}
		}

		private int getSearchResultCount(string resultContent)
		{
			Regex re;
			MatchCollection matches;

			re = new Regex(SEARCH_RESULT_COUNT_REGEX);
			matches = re.Matches(resultContent);

			if(matches.Count != 1)
			{
				_errorState = true;
				return 0;
			}

			return int.Parse(matches[0].Groups[1].Value);
		}

		public RawDataValue[] Values
		{
			get
			{
				RawDataValue dataValue;

				dataValue = new RawDataValue();
				//Todo: Get this from the server
				dataValue.Timestamp = DateTime.UtcNow;

				if (_errorState)
				{
					dataValue.Success = false;
					dataValue.FloatValue = null;
				}
				else
				{
					dataValue.Success = true;
					dataValue.FloatValue = (double?)_searchPosition;
					dataValue.Fuzzy = _fuzzy;
				}

				return new RawDataValue[] { dataValue };
			}
		}

		public Dictionary<int, object> Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		#endregion
	}
}
