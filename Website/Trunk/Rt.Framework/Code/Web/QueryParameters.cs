using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Rt.Framework.CommonControls.Web;

namespace Rt.Framework.Web
{
	/// <summary>
	///		Contains common query string parameters and functionality.
	/// </summary>
	public static class QueryParameters
	{
		public const string QUERY_TIME_RANGE = "r";
		public const string QUERY_START = "s";
		public const string QUERY_END = "e";
		public const string QUERY_TITLE = "t";
		public const string QUERY_DATASOURCE_LIST = "dl";

		public static Dictionary<string, object> ReadCommonQueryParameters(NameValueCollection queryString)
		{
			string paramString;
			Dictionary<string, object> parameters;

			parameters = new Dictionary<string, object>();

			if(queryString == null || queryString.Count == 0)
				return parameters;

			paramString = queryString[QUERY_TIME_RANGE];
			if (paramString != null)
			{
				int timeRangeVal;

				timeRangeVal = int.Parse(paramString);
				parameters.Add(QUERY_TIME_RANGE, (DateRangeSelector.DateRanges) timeRangeVal);
			}

			paramString = queryString[QUERY_START];
			if (paramString != null)
			{
				DateTime st;

				st = DateTime.Parse(paramString);
				parameters.Add(QUERY_START, st);
			}

			paramString = queryString[QUERY_END];
			if (paramString != null)
			{
				DateTime et;

				et = DateTime.Parse(paramString);
				parameters.Add(QUERY_END, et);
			}

			paramString = queryString[QUERY_TITLE];
			if (paramString != null)
			{
				parameters.Add(QUERY_TITLE, paramString);
			}

			paramString = queryString[QUERY_DATASOURCE_LIST];
			if (paramString != null)
			{
				parameters.Add(QUERY_DATASOURCE_LIST, ParseDatasourceList(paramString));
			}

			return parameters;
		}

		public static DatasourceIds[] ParseDatasourceList(string list)
		{
			string[] parts;
			List<DatasourceIds> idArray;

			parts = list.Split(new char[] {','});

			idArray = new List<DatasourceIds>(parts.Length);
			foreach(string currPart in parts)
			{
				DatasourceIds ids;

				ids = new DatasourceIds();

				if(currPart.Contains("."))
				{
					string[] subParts;

					subParts = currPart.Split(".".ToCharArray());
					ids.DatasourceId = int.Parse(subParts[0]);
					ids.SubTypeId = int.Parse(subParts[1]);
				}
				else
				{
					ids.DatasourceId = int.Parse(currPart);
					ids.SubTypeId = null;
				}

				idArray.Add(ids);
			}

			return idArray.ToArray();
		}

		public class DatasourceIds
		{
			public int DatasourceId;
			public int? SubTypeId;
		}
	}
}