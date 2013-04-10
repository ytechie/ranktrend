using System;
using Rt.Framework.Api.Model;

namespace Rt.Framework.Api
{
	public interface IRawDataRepository
	{
		/// <summary>
		///		Queries the raw data points.
		/// </summary>
		/// <param name="start">
		///		The inclusive start time to search for data.
		/// </param>
		/// <param name="end">
		///		The inclusive end time to search for data.
		/// </param>
		/// <param name="datasourceKeys">
		///		An array of <see cref="DatasourceKey"/> objects that specify
		///		a datasource ID and optional sub ID of each datasource that
		///		data should be retreived for.
		/// </param>
		/// <returns>
		///		An array of raw data points matching the filter criteria.
		/// </returns>
		DataPoint[] GetData(DateTime start, DateTime end, DatasourceKey[] datasourceKeys);

		/// <summary>
		///		A high performance way of adding values for multiple data points. If
		///		a datapoint is inserted with the same datasource and timestamp as an existing data
		///		point, the existing data point will be overwritten.
		/// </summary>
		/// <param name="datapoints">
		///		An array of datapoints to insert.
		/// </param>
		void SaveDataPoints(DataPoint[] datapoints);
	}
}
