using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using YTech.General.Web;
using DataStreams.Csv;
using Rt.Framework.Components;
using log4net;
using System.Reflection;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(3)]
	public class GoogleAdsense : BaseDataSource, IDataSource
	{
		private const string LOGIN_POST_STRING =
	"ltmpl=login&continue=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth&followup=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth&service=adsense&nui=3&fpui=3&ifr=true&rm=hide&ltmpl=login&hl=en-US&alwf=true&GA3T=aq1WSA0ODSc&Email={0}&Passwd={1}&null=Sign+in";

		private const string ADSENSE_LOGIN_URL =
			"https://www.google.com/accounts/ServiceLoginAuth?service=adsense&hl=en-US&ltmpl=login&ifr=true&passive=true&rm=hide&nui=3&alwf=true&continue=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth&followup=https%3A%2F%2Fwww.google.com%2Fadsense%2Fgaiaauth";

		private const string GOOGLE_LOGIN_URL = "https://www.google.com/accounts/ServiceLoginAuth";

		private const string ADSENSE_REPORT_URL =
			"https://www.google.com/adsense/report/aggregate";

		private stage _currentStage = stage.GetHiddenFields;
		private string[][] _loginFormHiddenFields;
		private string _authenticateRedirectUrl;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private bool _backLoad;

		/// <summary>
		///		These are the values that we'll read
		/// </summary>
		private RawDataValue[] _values;

		enum stage
		{
			GetHiddenFields,
			Authenticate,
			PostAuthenticateRedirect,
			RunReports
		}

		#region Datasource Parameters

		private string _username;
		private string _password;
		private string _channelName;

		#endregion

		#region Constructors

		public GoogleAdsense()
		{
		}

		public GoogleAdsense(bool backLoad)
		{
			_backLoad = backLoad;
		}

		#endregion

		#region Request/Response Steps

		#region GetHiddenFields

		/// <summary>
		///		This is phase 1 of the login process.  We need to make a request to
		///		the adsense login page, and get a list of all the hidden fields, so
		///		that we can post them back to the form just like a browser would.
		/// </summary>
		private SerializableWebRequest sendLoginFormHiddenFieldsRequest()
		{
			SerializableWebRequest req;

			req = new SerializableWebRequest();
			req.Url = ADSENSE_LOGIN_URL;

			return req;
		}

		private void processLoginFormHiddenFields(SerializableWebResponse resp)
		{
			MatchCollection matches;

			matches = Regex.Matches(resp.Content, "<input type=\"hidden\" name=\"(.*?)\" value=\"(.*?)\">");

			_loginFormHiddenFields = new string[matches.Count][];
			for (int i = 0; i < matches.Count; i++)
			{
				_loginFormHiddenFields[i] = new string[2];
				_loginFormHiddenFields[i][0] = matches[i].Groups[1].Value;
				_loginFormHiddenFields[i][1] = matches[i].Groups[2].Value;
			}
		}

		#endregion

		#region SendCredentials

		private SerializableWebRequest getSendCredentialsRequest()
		{
			StringBuilder postStringBuilder;
			SerializableWebRequest req;

			postStringBuilder = new StringBuilder();

			//Append all of the hidden field name/value pairs
			for (int i = 0; i < _loginFormHiddenFields.Length; i++)
			{
				if (i > 0)
					postStringBuilder.Append("&");

				postStringBuilder.Append(_loginFormHiddenFields[i][0] + "=" + HttpUtility.UrlEncode(_loginFormHiddenFields[i][1]));
			}

			//Append the username and password
			postStringBuilder.Append(
				string.Format("&Email={0}&Passwd={1}", HttpUtility.UrlEncode(_username), HttpUtility.UrlEncode(_password)));

			//Append some other stuff
			postStringBuilder.Append("&null=Sign+in");

			//Set up the request
			req = new SerializableWebRequest();
			req.Url = GOOGLE_LOGIN_URL;
			req.PostData = postStringBuilder.ToString();

			return req;
		}

		private void processSendCredentialsResponse(SerializableWebResponse resp)
		{
			Regex reg;
			MatchCollection urlMatches;
			RegexOptions options;

			options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant;

			reg = new Regex("<a href=\"(https://.*?)\"", options);

			urlMatches = reg.Matches(resp.Content);

			//Check if we found the URL to post to
			if (urlMatches.Count == 0)
				throw new ApplicationException("Unable to find redirect URL in Google Adsense authetication response");

			_authenticateRedirectUrl = urlMatches[0].Groups[1].Value;
		}

		#endregion

		#region Post Authenticate

		private SerializableWebRequest getPostAuthenticateRequest()
		{
			SerializableWebRequest req;

			req = new SerializableWebRequest();
			req.Url = _authenticateRedirectUrl;

			return req;
		}

		private void processPostAuthenticateResponse(SerializableWebResponse resp)
		{
			//The login process is now complete
		}

		#endregion

		#endregion

		private void processParameters()
		{
			_username = (string)_parameters[1];
			_password = (string)_parameters[2];

			if(_parameters.Count > 2)
				_channelName = (string)_parameters[3];
		}

		#region Run Report

		private SerializableWebRequest getRunReportRequest()
		{
			SerializableWebRequest request;
			string reportUrl;

			request = new SerializableWebRequest();

			//Get yesterdays revenues
			if (_backLoad)
			{
				throw new NotImplementedException("Backloading adsense data is not yet supported");
			}
			else
			{
				_log.Debug("The backload option was not set, so data will be retrieve for yesterday only");
				reportUrl = GenerateReportUrl(DateTime.Now.AddDays(-1).Date, DateTime.Now.AddDays(-1).Date);
			}
			
			request.Url = reportUrl;

			return request;
		}

		private void processReportResponse(SerializableWebResponse response)
		{
			_values = ParseAdsenseCsvReport(response.Content, _channelName);
		}

		public static RawDataValue[] ParseAdsenseCsvReport(string report, string channelFilter)
		{
			CsvReader csvReader;
			StringReader sr;
			DataTable data;

			using (sr = new StringReader(report))
			{
				using (csvReader = new CsvReader(sr, '\t'))
				{
					data = csvReader.ReadToEnd();
				}
			}

			return ProcessAdsenseData(data, channelFilter);
		}

		/// <summary>
		///		Processes the data that is parsed from the standard
		///		adsense CSV format.
		/// </summary>
		/// <param name="adsenseData">
		///		A data table that represents the adsense data.  This is usually
		///		created from the CSV export format.
		/// </param>
		/// <param name="channelName">
		///		If specified, the name of the channel to filter the data by.
		/// </param>
		/// <returns>
		///		An array of <see cref="RawDataValue" /> objects that are ready
		///		to be inserted into the database.  NOTE: The datasource and the
		///		sub type object are NOT available in the raw data values.  Only
		///		the ID of the sub type is set.
		/// </returns>
		public static RawDataValue[] ProcessAdsenseData(DataTable adsenseData, string channelName)
		{
			List<RawDataValue> dataValues;

			//Only get data for the correct channel, if specified
			if (!string.IsNullOrEmpty(channelName) && adsenseData.Columns.Contains("Channel"))
			{
				for (int i = adsenseData.Rows.Count - 1; i > 0; i--)
				{
					if (!adsenseData.Rows[i]["Channel"].Equals(channelName))
						adsenseData.Rows.Remove(adsenseData.Rows[i]);
				}
			}

			dataValues = new List<RawDataValue>();

			dataValues.AddRange(RawDataValue.GetRawDataForBulkInsert(adsenseData, "Date", "Earnings", 0, 1));
			dataValues.AddRange(RawDataValue.GetRawDataForBulkInsert(adsenseData, "Date", "Page impressions", 0, 2));
			dataValues.AddRange(RawDataValue.GetRawDataForBulkInsert(adsenseData, "Date", "Clicks", 0, 4));

			return dataValues.ToArray();
		}

		/// <summary>
		///		Generates a URL for retrieving an AdSense report based on the specified criteria.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static string GenerateReportUrl(DateTime start, DateTime end)
		{
			UrlBuilder url;

			url = new UrlBuilder(ADSENSE_REPORT_URL);

			url.Parameters.AddParameter("dateRange.dateRangeType", "custom");

			//Start time
			url.Parameters.AddParameter("dateRange.customDate.start.month", start.Month);
			url.Parameters.AddParameter("dateRange.customDate.start.day", start.Day);
			url.Parameters.AddParameter("dateRange.customDate.start.year", start.Year);

			//End time
			url.Parameters.AddParameter("dateRange.customDate.end.month", end.Month);
			url.Parameters.AddParameter("dateRange.customDate.end.day", end.Day);
			url.Parameters.AddParameter("dateRange.customDate.end.year", end.Year);

			//We want the data in CSV format
			url.Parameters.AddParameter("outputFormat", "TSV_EXCEL");

			return url.ToString();
		}

		#endregion

		#region IDataSource Members

		public SerializableWebRequest GetNextRequest()
		{
			//Check if we already returned a value
			if (_values != null)
				return null;

			switch (_currentStage)
			{
				case stage.GetHiddenFields:
					processParameters();
					return sendLoginFormHiddenFieldsRequest();
				case stage.Authenticate:
					return getSendCredentialsRequest();
				case stage.PostAuthenticateRedirect:
					return getPostAuthenticateRequest();
				case stage.RunReports:
					return getRunReportRequest();
				default:
					throw new NotImplementedException();
			}
		}

		public void SetResponse(SerializableWebResponse response)
		{
			switch (_currentStage)
			{
				case stage.GetHiddenFields:
					//Advance to the next stage
					_currentStage = stage.Authenticate;
					processLoginFormHiddenFields(response);
					break;
				case stage.Authenticate:
					//Advance to the next stage
					_currentStage = stage.PostAuthenticateRedirect;
					processSendCredentialsResponse(response);
					break;
				case stage.PostAuthenticateRedirect:
					//Advance to the next stage
					_currentStage = stage.RunReports;
					processPostAuthenticateResponse(response);
					break;
				case stage.RunReports:
					processReportResponse(response);
					break;
			}
		}

		public RawDataValue[] Values
		{
			get { return _values; }
		}
		
		#endregion
	}
}
