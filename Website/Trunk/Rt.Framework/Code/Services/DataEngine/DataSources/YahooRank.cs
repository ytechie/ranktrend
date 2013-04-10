using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Web;
using System.Globalization;
using YTech.General.Web.Seo.RankChecking.Yahoo;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(1)]
	public class YahooRank : IDataSource
	{
		private const string YAHOO_API_URL = "http://api.search.yahoo.com/WebSearchService/V1/webSearch?appid={0}&query={1}&type={2}&results={3}&start={4}&format={5}&adult_ok={6}&similar_ok={7}&language={8}";
		private const string API_KEY = "RankTrend";

		private string _url;
		private Dictionary<int, object> _parameters;
		private string _searchPhrase;
		private bool _requestMade;
		private int? _searchPosition;

		/// <summary>
		///		Creates a new instance of the <see cref="YahooRank" /> class.
		/// </summary>
		public YahooRank()
		{
			_requestMade = false;
		}

		private void readParameters()
		{
			_searchPhrase = (string)_parameters[1];
		}

		//Yahoo.API.WebSearchResponse.ResultSet resultSet = yahoo.WebSearch("YahooExample", "site:www.mgbrown.com", "all", 10, 1, "any", true, true, "en");

		/// <summary>
		///		Builds a URL that can be used to query the Yahoo search API
		/// </summary>
		/// <param name="appId">
		///		The application ID assigned by Yahoo.
		/// </param>
		/// <param name="query">
		/// 	The query to search for. This query supports the full search
		/// 	language of Yahoo! Search, including meta keywords.
		/// </param>
		/// <param name="type">
		///		The kind of search to submit:
		///			* all returns results with all query terms.
    ///			* any returns results with one or more of the query terms.
    ///			* phrase returns results containing the query terms as a phrase.
		/// </param>
		/// <param name="results">
		///		The number of results to return, up to 100.
		/// </param>
		/// <param name="start">
		///		The starting result position to return (1-based). The finishing
		///		position (start + results - 1) cannot exceed 1000.
		/// </param>
		/// <param name="format">
		///		Specifies the kind of file to search for. (xml, xls, pdf, ppt, etc).
		///		The default is "any".
		/// </param>
		/// <param name="adultOk">
		///		Specifies whether to allow results with adult content.
		/// </param>
		/// <param name="similarOk">
		///		Specifies whether to allow multiple results with similar content.
		/// </param>
		/// <param name="language">
		///		The language the results are written in, which defaults to all languages.
		/// </param>
		/// <returns></returns>
		/// <seealso href="http://developer.yahoo.com/search/web/V1/webSearch.html">Yahoo API Reference</seealso>
		private string GetApiRequestUrl(string appId, string query, string type, short results, int start, string format,
													 bool adultOk, bool similarOk, string language)
		{
			return string.Format(YAHOO_API_URL,
				appId, HttpUtility.UrlEncode(query, Encoding.UTF8), type, results, start, format, adultOk ? "1" : "0",
					similarOk ? "1" : "0", language);
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
			request.Url = this.GetApiRequestUrl(API_KEY, _searchPhrase, "phrase", 100, 1, "any", true, false, "");

			return request;
		}

		/// <summary>
		///		Parses the response and stores the results in the database.
		/// </summary>
		/// <remarks>
		///		This should be called once the request has been made.
		/// </remarks>
		/// <param name="response">
		///		The response that was recieved as a result of making the
		///		request from <see cref="SerializableWebRequest"/>.
		///	</param>
		public void SetResponse(SerializableWebResponse response)
		{
			XmlSerializer serializer;
			StringReader sr;
			YTech.General.Web.Seo.RankChecking.Yahoo.ResultSet rs;

			using (sr = new StringReader(response.Content))
			{
				serializer = new XmlSerializer(typeof(ResultSet));
				rs = (ResultSet)serializer.Deserialize(sr);
			}

			for(int i = 0; i < rs.Result.Length; i++)
			{
				if (rs.Result[i].Url.StartsWith(Url, true, CultureInfo.InvariantCulture))
				{
					//We found a match!

					//Remember, item 0 in the array is search result #1
					_searchPosition = i + 1;
					break;
				}
			}
		}

		public RawDataValue[] Values
		{
			get
			{
				RawDataValue dataValue;

				dataValue = new RawDataValue();
				//Todo: Get this from the server
				dataValue.Timestamp = DateTime.UtcNow;
				dataValue.Success = true;
				dataValue.FloatValue = (double?)_searchPosition;

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
