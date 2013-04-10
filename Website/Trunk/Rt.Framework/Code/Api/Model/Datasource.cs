namespace Rt.Framework.Api.Model
{
	public class Datasource
	{
		public DatasourceKey Key { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DatasourceParameter[] Parameters { get; set; }
	}
}
