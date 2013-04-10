namespace Rt.Framework.Api.Model
{
	/// <summary>
	///		A datasource id/sub-id pair that is associated with
	///		a data series.
	/// </summary>
	public class DatasourceKey
	{
		/// <summary>
		///		The primary ID of a source of data.
		/// </summary>
		public int DatasourceId{ get; set;}

		/// <summary>
		///		If supplied, retreives a series of data for
		///		a sub-component of a datasource. Most datasources
		///		do not have any sub-datasources.
		/// </summary>
		public int? SubId { get; set; }
	}
}
