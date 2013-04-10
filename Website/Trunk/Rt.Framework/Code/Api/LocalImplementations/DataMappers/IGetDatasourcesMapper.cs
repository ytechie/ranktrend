using System.Collections.Generic;
using System.Data;
using Rt.Framework.Api.Model;

namespace Rt.Framework.Api.LocalImplementations.DataMappers
{
	public interface IGetDatasourcesMapper
	{
		List<Datasource> MapDataset(DataSet ds);
	}
}