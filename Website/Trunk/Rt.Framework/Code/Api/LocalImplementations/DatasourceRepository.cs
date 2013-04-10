using System.Collections.Generic;
using System.Data;
using NHibernate.Expression;
using Rt.Framework.Api.LocalImplementations.DataMappers;
using Rt.Framework.Api.Model;
using Rt.Framework.Components;
using YTech.Db;
using DatasourceParameter=Rt.Framework.Api.Model.DatasourceParameter;
using DbConnection=YTech.Db.SqlServer.DbConnection;

namespace Rt.Framework.Api.LocalImplementations
{
	public class DatasourceRepository : IDatasourceRepository
	{
		private DbConnection _dbConn;
		private ORManager _orm;
		private IGetDatasourcesMapper _getDatasourcesMapper;

		public DatasourceRepository(DbConnection dbConn, ORManager orm, IGetDatasourcesMapper getDatasourcesMapper)
		{
			_dbConn = dbConn;
			_orm = orm;

			_getDatasourcesMapper = getDatasourcesMapper;
		}

		public Datasource[] GetDatasources(string url, bool getParameters, int[] types)
		{
			var stringTypes = new List<string>();
			foreach (var currType in types)
				stringTypes.Add(currType.ToString());

			var cmd = _dbConn.GetStoredProcedureCommand("Api_GetDatasources");
			cmd.Parameters.AddWithValue("@Url", url);
			cmd.Parameters.AddWithValue("@GetParameters", getParameters);
			cmd.Parameters.AddWithValue("@TypeString", string.Join(",", stringTypes.ToArray()));

			var ds = _dbConn.GetDataSet(cmd);

			return _getDatasourcesMapper.MapDataset(ds).ToArray();
		}

		public void SaveDatasources(Datasource[] datasources)
		{
			throw new System.NotImplementedException();
		}

		public void DeleteDatasource(int datasourceId)
		{
			throw new System.NotImplementedException();
		}

		public Datasource[] GetDatasourceTemplates()
		{
			throw new System.NotImplementedException();
		}

		public void ReplaceKeywords(string url, string[] keywords)
		{
			var cmd = _dbConn.GetStoredProcedureCommand("Api_ReplaceKeywords");
			cmd.Parameters.AddWithValue("@Url", url);
			cmd.Parameters.AddWithValue("@Keywords", string.Join(",", keywords));

			_dbConn.ExecuteNonQuery(cmd);
		}
	}
}
