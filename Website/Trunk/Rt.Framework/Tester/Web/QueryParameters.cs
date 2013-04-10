using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using Rt.Framework.CommonControls.Web;

namespace Rt.Framework.Web
{
	[TestFixture]
	public class QueryParameters_Tester
	{
		[Test]
		public void ParseDatasourceList()
		{
			QueryParameters.DatasourceIds[] list;

			list = QueryParameters.ParseDatasourceList("5.6,4,3,6.0");
			Assert.AreEqual(5, list[0].DatasourceId);
			Assert.AreEqual(6, list[0].SubTypeId.Value);

			Assert.AreEqual(4, list[1].DatasourceId);
			Assert.AreEqual(null, list[1].SubTypeId);

			Assert.AreEqual(3, list[2].DatasourceId);
			Assert.AreEqual(null, list[2].SubTypeId);

			Assert.AreEqual(6, list[3].DatasourceId);
			Assert.AreEqual(0, list[3].SubTypeId.Value);
		}

		#region ReadCommonQueryParameters

		[Test]
		public void ReadCommonQueryParameters()
		{
			Dictionary<string, object> parameters;

			parameters = QueryParameters.ReadCommonQueryParameters(null);
			Assert.AreEqual(0, parameters.Count);
		}

		[Test]
		public void ReadCommonQueryParameters_TimeRange()
		{
			Dictionary<string, object> parameters;
			NameValueCollection q = new NameValueCollection();

			q.Add(QueryParameters.QUERY_TIME_RANGE, ((int)DateRangeSelector.DateRanges.Last6Months).ToString());

			parameters = QueryParameters.ReadCommonQueryParameters(q);
			Assert.AreEqual(1, parameters.Count);
			Assert.AreEqual(DateRangeSelector.DateRanges.Last6Months, parameters[QueryParameters.QUERY_TIME_RANGE]);
		}

		[Test]
		public void ReadCommonQueryParameters_StartTime()
		{
			Dictionary<string, object> parameters;
			NameValueCollection q = new NameValueCollection();

			q.Add(QueryParameters.QUERY_START, "1/2/05");

			parameters = QueryParameters.ReadCommonQueryParameters(q);
			Assert.AreEqual(1, parameters.Count);
			Assert.AreEqual(DateTime.Parse("1/2/05"), parameters[QueryParameters.QUERY_START]);
		}

		[Test]
		public void ReadCommonQueryParameters_EndTime()
		{
			Dictionary<string, object> parameters;
			NameValueCollection q = new NameValueCollection();

			q.Add(QueryParameters.QUERY_END, "1/6/05");

			parameters = QueryParameters.ReadCommonQueryParameters(q);
			Assert.AreEqual(1, parameters.Count);
			Assert.AreEqual(DateTime.Parse("1/6/05"), parameters[QueryParameters.QUERY_END]);
		}

		[Test]
		public void ReadCommonQueryParameters_Title()
		{
			Dictionary<string, object> parameters;
			NameValueCollection q = new NameValueCollection();

			q.Add(QueryParameters.QUERY_TITLE, "test title");

			parameters = QueryParameters.ReadCommonQueryParameters(q);
			Assert.AreEqual(1, parameters.Count);
			Assert.AreEqual("test title", parameters[QueryParameters.QUERY_TITLE]);
		}

		[Test]
		public void ReadCommonQueryParameters_DatasourceList()
		{
			Dictionary<string, object> parameters;
			NameValueCollection q = new NameValueCollection();
			QueryParameters.DatasourceIds[] list;

			q.Add(QueryParameters.QUERY_DATASOURCE_LIST, "5,7,8.6,4");

			parameters = QueryParameters.ReadCommonQueryParameters(q);
			Assert.AreEqual(1, parameters.Count);
			list = (QueryParameters.DatasourceIds[]) parameters[QueryParameters.QUERY_DATASOURCE_LIST];
			Assert.AreEqual(5, list[0].DatasourceId);
			Assert.AreEqual(null, list[0].SubTypeId);

			Assert.AreEqual(7, list[1].DatasourceId);
			Assert.AreEqual(null, list[1].SubTypeId);

			Assert.AreEqual(8, list[2].DatasourceId);
			Assert.AreEqual(6, list[2].SubTypeId.Value);

			Assert.AreEqual(4, list[3].DatasourceId);
			Assert.AreEqual(null, list[3].SubTypeId);
		}

		[Test]
		public void ReadCommonQueryParameters_Multiple()
		{
			Dictionary<string, object> parameters;
			NameValueCollection q = new NameValueCollection();

			q.Add(QueryParameters.QUERY_START, "1/2/05");
			q.Add(QueryParameters.QUERY_END, "1/6/05");

			parameters = QueryParameters.ReadCommonQueryParameters(q);
			Assert.AreEqual(2, parameters.Count);
			Assert.AreEqual(DateTime.Parse("1/2/05"), parameters[QueryParameters.QUERY_START]);
			Assert.AreEqual(DateTime.Parse("1/6/05"), parameters[QueryParameters.QUERY_END]);
		}

		#endregion
	}
}
