using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(9)]
	public class Digg : IDataSource
	{
		private Dictionary<int, object> _parameters;
		private string _diggUrl;
		private string _url;
		private int _diggCount;
		private int _commentCount;
		private bool _finished;
		private bool _errorState;

		private const string REGEX_DIGGS = @"<li class=\""digg-count\"" id=\""main1\""><span id=\""diggs1\""><strong id=\""diggs-strong-1\"">(\d+)</strong> diggs</span></li>";
		private const string REGEX_COMMENT_COUNT = @"<span>Comments \(<strong id=""total-cmts"">(\d+)</strong>\)</span>";

		public Digg()
		{
			_parameters = new Dictionary<int, object>();
		}

		private void processParamters()
		{
			_diggUrl = (string)_parameters[1];
		}

		#region IDataSource Members

		public SerializableWebRequest GetNextRequest()
		{
			SerializableWebRequest req;

			if (_finished)
				return null;

			processParamters();

			req = new SerializableWebRequest();
			req.Url = _diggUrl;

			return req;
		}

		public void SetResponse(SerializableWebResponse response)
		{
			Regex rx;
			MatchCollection matches;
			string captureString;

			rx = new Regex(REGEX_DIGGS);
			matches = rx.Matches(response.Content);
			if (matches.Count == 0)
			{
				_errorState = true;
				return;
			}
			captureString = matches[0].Groups[1].Value;
			_diggCount = int.Parse(captureString);

			rx = new Regex(REGEX_COMMENT_COUNT);
			matches = rx.Matches(response.Content);
			if (matches.Count == 0)
			{
				_errorState = true;
				return;
			}
			_commentCount = int.Parse(matches[0].Groups[1].Value);

			_finished = true;
		}

		public RawDataValue[] Values
		{
			get
			{
				RawDataValue[] values;

				values = new RawDataValue[2];

				values[0] = new RawDataValue();
				values[0].DatasourceSubTypeId = 5;
				values[0].Success = !_errorState;
				values[0].FloatValue = _diggCount;
				values[0].Timestamp = DateTime.UtcNow;

				values[1] = new RawDataValue();
				values[1].DatasourceSubTypeId = 6;
				values[1].Success = !_errorState;
				values[1].FloatValue = _commentCount;
				values[1].Timestamp = DateTime.UtcNow;

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
			get{ return _url; }
			set { _url = value; }
		}

		#endregion
	}
}
