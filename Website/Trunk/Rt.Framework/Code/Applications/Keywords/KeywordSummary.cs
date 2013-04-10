using System.Collections.Generic;
using System.Data;

namespace Rt.Framework.Applications.Keywords
{
	public static class KeywordSummary
	{
		/// <summary>
		///		Combines lists of keywords into a table with keywords on the left, search
		///		engines on the top, and booleans for values.
		/// </summary>
		/// <remarks>
		///		The first column is used as the keyword list.  It's header is used as the
		///		keyword header in the output table.
		/// </remarks>
		/// <returns></returns>
		public static DataTable CombineKeywordLists(DataTable[] keywordTables)
		{
			Dictionary<string, List<bool>> keywords;
			DataTable results;

			keywords = new Dictionary<string, List<bool>>();

			for (int i = 0; i < keywordTables.Length; i++)
			{
				for (int j = 0; j < keywordTables[i].Rows.Count; j++)
				{
					string keyword;
					List<bool> currBools;

					keyword = ((string) keywordTables[i].Rows[j][0]).ToLower();

					if (!keywords.TryGetValue(keyword, out currBools))
					{
						currBools = new List<bool>(keywordTables.Length);
						for (int k = 0; k < keywordTables.Length; k++)
							currBools.Add(false);

						keywords.Add(keyword, currBools);
					}

					currBools[i] = true;
				}
			}

			//Initialize the structure of the output table
			results = new DataTable();

			results.Columns.Add(new DataColumn(keywordTables[0].Columns[0].ColumnName, typeof (string)));
			for (int i = 0; i < keywordTables.Length; i++)
			{
				DataColumn newCol;

				newCol = new DataColumn(keywordTables[i].TableName);
				newCol.DataType = typeof (bool);
				results.Columns.Add(newCol);
			}

			//Now create the output data table based on they keyword list
			foreach (string currKeyword in keywords.Keys)
			{
				object[] rowValues;
				List<bool> keywordBools;

				keywordBools = keywords[currKeyword];

				rowValues = new object[keywordTables.Length + 1];
				rowValues[0] = currKeyword;

				for (int i = 0; i < keywordBools.Count; i++)
					rowValues[i + 1] = keywordBools[i];

				results.Rows.Add(rowValues);
			}

			return results;
		}
	}
}