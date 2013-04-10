using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NUnit.Framework;

namespace Rt.Framework.Applications.Keywords
{
	[TestFixture]
	public class KeywordSummary_Tester
	{
		[Test]
		public void CombineKeywordLists()
		{
			DataTable dt;

			dt = KeywordSummary.CombineKeywordLists(new DataTable[] { getKeywordTable1(), getKeywordTable2() });

			Assert.AreEqual(5, dt.Rows.Count);
			Assert.AreEqual(3, dt.Columns.Count);

			Assert.AreEqual("dog", dt.Rows[0][0]);
			Assert.AreEqual("cat", dt.Rows[1][0]);
			Assert.AreEqual("rabbit", dt.Rows[2][0]);
			Assert.AreEqual("horse", dt.Rows[3][0]);
			Assert.AreEqual("moose", dt.Rows[4][0]);

			Assert.AreEqual(true, dt.Rows[0][1]);
			Assert.AreEqual(true, dt.Rows[1][1]);
			Assert.AreEqual(true, dt.Rows[2][1]);
			Assert.AreEqual(true, dt.Rows[3][1]);
			Assert.AreEqual(false, dt.Rows[4][1]);

			Assert.AreEqual(true, dt.Rows[0][2]);
			Assert.AreEqual(true, dt.Rows[1][2]);
			Assert.AreEqual(false, dt.Rows[2][2]);
			Assert.AreEqual(false, dt.Rows[3][2]);
			Assert.AreEqual(true, dt.Rows[4][2]);
		}

		/// <summary>
		///		Verify that the columns are named after the appropriate data
		///		table names.
		/// </summary>
		[Test]
		public void CombineKeywordLists2()
		{
			DataTable dt;

			dt = KeywordSummary.CombineKeywordLists(new DataTable[] { getKeywordTable1(), getKeywordTable2() });

			Assert.AreEqual("Sample1", dt.Columns[1].ColumnName);
			Assert.AreEqual("Sample2", dt.Columns[2].ColumnName);
		}

		/// <summary>
		///		Verify that case is ignored
		/// </summary>
		[Test]
		public void CombineKeywordLists3()
		{
			DataTable dt;

			dt = KeywordSummary.CombineKeywordLists(new DataTable[] { getKeywordTable1(), getKeywordTable3() });

			Assert.AreEqual(5, dt.Rows.Count);
			Assert.AreEqual(3, dt.Columns.Count);

			Assert.AreEqual("dog", dt.Rows[0][0]);
			Assert.AreEqual("cat", dt.Rows[1][0]);
			Assert.AreEqual("rabbit", dt.Rows[2][0]);
			Assert.AreEqual("horse", dt.Rows[3][0]);
			Assert.AreEqual("moose", dt.Rows[4][0]);

			Assert.AreEqual(true, dt.Rows[0][1]);
			Assert.AreEqual(true, dt.Rows[1][1]);
			Assert.AreEqual(true, dt.Rows[2][1]);
			Assert.AreEqual(true, dt.Rows[3][1]);
			Assert.AreEqual(false, dt.Rows[4][1]);

			Assert.AreEqual(true, dt.Rows[0][2]);
			Assert.AreEqual(true, dt.Rows[1][2]);
			Assert.AreEqual(false, dt.Rows[2][2]);
			Assert.AreEqual(false, dt.Rows[3][2]);
			Assert.AreEqual(true, dt.Rows[4][2]);
		}

		#region Helpers

		private DataTable getKeywordStructure()
		{
			DataTable dt;

			dt = new DataTable();
			dt.Columns.Add("Keyword");

			return dt;
		}

		private DataTable getKeywordTable1()
		{
			DataTable dt;

			dt = getKeywordStructure();
			dt.TableName = "Sample1";
			dt.Rows.Add(new object[] { "dog" });
			dt.Rows.Add(new object[] { "cat" });
			dt.Rows.Add(new object[] { "rabbit" });
			dt.Rows.Add(new object[] { "horse" });

			return dt;
		}

		private DataTable getKeywordTable2()
		{
			DataTable dt;

			dt = getKeywordStructure();
			dt.TableName = "Sample2";
			dt.Rows.Add(new object[] { "dog" });
			dt.Rows.Add(new object[] { "cat" });
			dt.Rows.Add(new object[] { "moose" });

			return dt;
		}

		private DataTable getKeywordTable3()
		{
			DataTable dt;

			dt = getKeywordStructure();
			dt.TableName = "Sample2";
			dt.Rows.Add(new object[] { "DOG" });
			dt.Rows.Add(new object[] { "CaT" });
			dt.Rows.Add(new object[] { "moose" });

			return dt;
		}

		#endregion
	}
}
