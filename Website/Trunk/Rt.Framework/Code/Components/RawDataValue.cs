using System;
using System.Collections.Generic;
using System.Data;

namespace Rt.Framework.Components
{
	public class RawDataValue
	{
		private int _configuredDatasourceId;
		private int? _datasourceSubTypeId;
		private double? _floatValue;
		private bool _fuzzy;
		private long? _id;
		private bool _success;
		private DateTime _timestamp;

		public long? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int ConfiguredDatasourceId
		{
			get { return _configuredDatasourceId; }
			set { _configuredDatasourceId = value; }
		}

		public int? DatasourceSubTypeId
		{
			get { return _datasourceSubTypeId; }
			set { _datasourceSubTypeId = value; }
		}

		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		public double? FloatValue
		{
			get { return _floatValue; }
			set { _floatValue = value; }
		}

		public bool Success
		{
			get { return _success; }
			set { _success = value; }
		}

		public bool Fuzzy
		{
			get { return _fuzzy; }
			set { _fuzzy = value; }
		}

		/// <summary>
		///		Analyzes a table and extracts the raw data values that
		///		can be bulk inserted into the database.
		/// </summary>
		/// <param name="data">
		///		A data table usually representing a CSV file.
		/// </param>
		/// <param name="timestampColumnName">
		///		The name of the column to read the timestamps from.  This
		///		column is assumed to be of type string.
		/// </param>
		/// <param name="valueColumnName">
		///		The name of the column to read the values from.  This
		///		column is assumed to be of type string, and
		///		will be cast to a float.
		/// </param>
		/// <param name="configuredDatasourceId">
		///		The configured datasource ID that should be assigned to the
		///		new raw data values.
		/// </param>
		/// <param name="datasourceSubTypeId">
		///		The datasource sub type that should be assigned to the new
		///		raw data values.
		/// </param>
		/// <returns>
		///		An array of <see cref="RawDataValue" /> objects that are ready
		///		to be inserted into the database.  NOTE: The datasource and the
		///		sub type object are NOT available in the raw data values.  Only
		///		the ID fields are set.
		/// </returns>
		public static RawDataValue[] GetRawDataForBulkInsert(DataTable data, string timestampColumnName,
		                                                     string valueColumnName,
		                                                     int configuredDatasourceId, int? datasourceSubTypeId)
		{
			bool isValidTimestamp;
			bool isValidValue;
			DateTime timestamp;
			double value;
			RawDataValue dataValue;
			List<RawDataValue> dataValues;

			dataValues = new List<RawDataValue>();
			for (int i = 0; i < data.Rows.Count; i++)
			{
				dataValue = new RawDataValue();

				//Check if the row is a valid row
				isValidTimestamp = DateTime.TryParse((string) data.Rows[i][timestampColumnName], out timestamp);
				isValidValue = double.TryParse((string) data.Rows[i][valueColumnName], out value);

				//Only add rows that have valid timestamps
				if (isValidTimestamp)
				{
					dataValue.Id = 0;
					dataValue.ConfiguredDatasourceId = configuredDatasourceId;
					dataValue.DatasourceSubTypeId = datasourceSubTypeId;
					dataValue.Timestamp = timestamp;
					if (isValidValue)
					{
						dataValue.FloatValue = value;
						dataValue.Success = true;
					}
					else
					{
						dataValue.FloatValue = null;
						dataValue.Success = false;
					}

					dataValues.Add(dataValue);
				}
			}

			return dataValues.ToArray();
		}
	}
}