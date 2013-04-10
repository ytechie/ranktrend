using System.Data;
using NUnit.Framework;

namespace Rt.Framework.Api.LocalImplementations.DataMappers
{
	[TestFixture]
	public class GetDatasourcesMapper_Tester
	{
		private GetDatasourcesMapper _gdm;

		[SetUp]
		public void SetUp()
		{
			_gdm = new GetDatasourcesMapper();
		}

		[Test]
		public void Process_Datasource_List_From_DataSet()
		{
			var ds = getBaseTestDataSet();

			var datasources = _gdm.MapDataset(ds);
			Assert.AreEqual(2, datasources.Count);

			Assert.AreEqual(4, datasources[0].Key.DatasourceId);
			Assert.AreEqual("ds1", datasources[0].Name);
			Assert.AreEqual("desc1", datasources[0].Description);

			Assert.AreEqual(6, datasources[1].Key.DatasourceId);
			Assert.AreEqual("ds2", datasources[1].Name);
			Assert.AreEqual("desc2", datasources[1].Description);
		}

		[Test]
		public void Process_Parameter_List_From_DataSet()
		{
			var ds = getBaseTestDataSet();

			var parameterTable = ds.Tables.Add();
			parameterTable.Columns.Add("Id", typeof(int));
			parameterTable.Columns.Add("Description", typeof(string));
			parameterTable.Columns.Add("Value", typeof(string));

			parameterTable.Rows.Add(4, "param1", "value1");
			parameterTable.Rows.Add(4, "param2", "value2");
			parameterTable.Rows.Add(6, "param1", "value1");

			var datasources = _gdm.MapDataset(ds);

			Assert.AreEqual(2, datasources[0].Parameters.Length);
			Assert.AreEqual("param1", datasources[0].Parameters[0].Name);
			Assert.AreEqual("value1", datasources[0].Parameters[0].Value);
			Assert.AreEqual("param2", datasources[0].Parameters[1].Name);
			Assert.AreEqual("value2", datasources[0].Parameters[1].Value);

			Assert.AreEqual(1, datasources[1].Parameters.Length);
			Assert.AreEqual("param1", datasources[1].Parameters[0].Name);
			Assert.AreEqual("value1", datasources[1].Parameters[0].Value);
		}

		private static DataSet getBaseTestDataSet()
		{
			var ds = new DataSet();
			var datasourceTable = ds.Tables.Add();
			datasourceTable.Columns.Add("Id", typeof(int));
			datasourceTable.Columns.Add("Name", typeof(string));
			datasourceTable.Columns.Add("Description", typeof(string));

			datasourceTable.Rows.Add(4, "ds1", "desc1");
			datasourceTable.Rows.Add(6, "ds2", "desc2");

			return ds;
		}
	}
}
