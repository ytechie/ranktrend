using System.Collections.Generic;
using System.Data;
using Rt.Framework.Api.Model;

namespace Rt.Framework.Api.LocalImplementations.DataMappers
{
	public class GetDatasourcesMapper : IGetDatasourcesMapper
	{
		public List<Datasource> MapDataset(DataSet ds)
		{
			var datasources = new Dictionary<int, Datasource>();

			foreach (DataRow currRow in ds.Tables[0].Rows)
			{
				var newDatasource = new Datasource
				                    	{
				                    		Name = ((string) currRow["Name"]),
				                    		Description = ((string) currRow["Description"]),
				                    		Key = new DatasourceKey {DatasourceId = (int) currRow["Id"]}
				                    	};

				datasources.Add(newDatasource.Key.DatasourceId, newDatasource);
			}

			if (ds.Tables.Count > 1)
			{
				//There are parameter to process
				foreach (DataRow currRow in ds.Tables[1].Rows)
				{
					var datasourceId = (int)currRow["Id"];
					var newParameter = new DatasourceParameter();
					List<DatasourceParameter> parameters;

					if(datasources[datasourceId].Parameters == null)
						parameters = new List<DatasourceParameter>();
					else
						parameters = new List<DatasourceParameter>(datasources[datasourceId].Parameters);

					newParameter.Name = (string)currRow["Description"];
					newParameter.Value = (string)currRow["Value"];
					parameters.Add(newParameter);

					//BIG BUG!!! - this only uses the last parameter
					datasources[datasourceId].Parameters = parameters.ToArray();
				}
			}

			var datasourceList = new List<Datasource>();
			foreach (var currDatasourceKeys in datasources.Keys)
				datasourceList.Add(datasources[currDatasourceKeys]);

			return datasourceList;
		}
	}
}
