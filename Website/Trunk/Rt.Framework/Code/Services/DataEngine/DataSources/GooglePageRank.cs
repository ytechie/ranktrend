using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using log4net;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine.DataSources
{
	[DatasourceAttribute(5)]
	public class GooglePageRank : IDataSource
	{
		private int? _pageRank;
		private Dictionary<int, object> _parameters;
		private string _url;
		private YTech.General.Web.Seo.GooglePageRank _gpr;
		private bool _requestMade = false;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public GooglePageRank()
		{
			_gpr = new YTech.General.Web.Seo.GooglePageRank();
		}

		#region IDataSource Members

		public SerializableWebRequest GetNextRequest()
		{
			SerializableWebRequest request;

			if (_requestMade)
				return null;

			_log.Debug("Setting 'finished' flag to true");
			_requestMade = true;

			request = new SerializableWebRequest();
			request.Url = _gpr.GetPageRankURL(_url, true);

			_log.DebugFormat("Created Google PageRank request: '{0}'", request.ToString());

			return request;
		}

		public void SetResponse(SerializableWebResponse response)
		{
			_log.DebugFormat("Parsing PageRank response: '{0}'", response.ToString());
			_pageRank = _gpr.ParseRankFromResult(response.Content);

			_log.DebugFormat("PageRank parsed as {0}", _pageRank);
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
				dataValue.FloatValue = (double?)_pageRank;

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
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		#endregion
	}
}
