using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(10)]
	public class Delicious : IDataSource
	{
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		///		{0} = The MD5 hash of the URL to view information on.
		/// </remarks>
		private const string DELICIOUS_URL = "http://del.icio.us/url/{0}";

		private const string REGEX_LINK_COUNT = @"this url has been saved by (\d+) (?:people|person)";

		private bool _requestMade;
		private string _url;
		private Dictionary<int, object> _parameters;
		private int? _linkCount;

		private static string getDeliciousUrl(string urlToCheck)
		{
			string md5;

			md5 = hashUrl(urlToCheck);

			return string.Format(DELICIOUS_URL, md5);
		}

		public static string FormatUrl(string url)
		{
			Uri u;

			u = new Uri(url);
			if (u.AbsolutePath == "/" && !url.EndsWith("/"))
				return url + "/";

			return url;
		}

		// Hash an input string and return the hash as
		// a 32 character hexadecimal string.
		private static string hashUrl(string input)
		{
			// Create a new instance of the MD5CryptoServiceProvider object.
			MD5 md5Hasher = MD5.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}

		public static int? GetLinkCount(string pageContent)
		{
			Regex rx;
			MatchCollection matches;

			if(pageContent.Contains("There is no del.icio.us history for this url"))
				return 0;

			rx = new Regex(REGEX_LINK_COUNT);
			matches = rx.Matches(pageContent);

			//Make sure the regex worked
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

			req.Url = getDeliciousUrl(FormatUrl(_url));

			_requestMade = true;

			return req;
		}

		public void SetResponse(SerializableWebResponse response)
		{
			_linkCount = GetLinkCount(response.Content);
		}

		public Rt.Framework.Components.RawDataValue[] Values
		{
			get
			{
				RawDataValue[] values;

				values = new RawDataValue[1];
				values[0] = new RawDataValue();
				values[0].Timestamp = DateTime.UtcNow; //Todo: get this from the database?
				values[0].FloatValue = _linkCount;

				if (_linkCount == null)
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
