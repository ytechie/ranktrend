using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rt.Framework.Applications.InteractiveReport
{
	[TestFixture]
	public class RankChartGenerator_Tester
	{
		[Test]
		public void GetDateLabelsTest()
		{
			DateTime[] dates;

			dates = RankChartGenerator.GetLabelDates(DateTime.Parse("1/1/07"), DateTime.Parse("1/10/07"), 5, 20);
			Assert.AreEqual(11, dates.Length);
			Assert.AreEqual(DateTime.Parse("1/1/07"), dates[0]);
			Assert.AreEqual(DateTime.Parse("1/2/07"), dates[1]);
			Assert.AreEqual(DateTime.Parse("1/3/07"), dates[2]);
			Assert.AreEqual(DateTime.Parse("1/4/07"), dates[3]);
			Assert.AreEqual(DateTime.Parse("1/5/07"), dates[4]);
			Assert.AreEqual(DateTime.Parse("1/6/07"), dates[5]);
			Assert.AreEqual(DateTime.Parse("1/7/07"), dates[6]);
			Assert.AreEqual(DateTime.Parse("1/8/07"), dates[7]);
			Assert.AreEqual(DateTime.Parse("1/9/07"), dates[8]);
			Assert.AreEqual(DateTime.Parse("1/10/07"), dates[9]);
			Assert.AreEqual(DateTime.Parse("1/11/07"), dates[10]);
		}

		[Test]
		public void GetDateLabelsTest2()
		{
			DateTime[] dates;

			dates = RankChartGenerator.GetLabelDates(DateTime.Parse("1/1/07"), DateTime.Parse("1/17/07"), 5, 15);
			Assert.AreEqual(9, dates.Length);
			Assert.AreEqual(DateTime.Parse("1/1/07"), dates[0]);
			Assert.AreEqual(DateTime.Parse("1/3/07"), dates[1]);
			Assert.AreEqual(DateTime.Parse("1/5/07"), dates[2]);
			Assert.AreEqual(DateTime.Parse("1/7/07"), dates[3]);
			Assert.AreEqual(DateTime.Parse("1/9/07"), dates[4]);
			Assert.AreEqual(DateTime.Parse("1/11/07"), dates[5]);
			Assert.AreEqual(DateTime.Parse("1/13/07"), dates[6]);
			Assert.AreEqual(DateTime.Parse("1/15/07"), dates[7]);
			Assert.AreEqual(DateTime.Parse("1/17/07"), dates[8]);
		}

		[Test]
		public void GetDateLabelsTest3()
		{
			DateTime[] dates;

			dates = RankChartGenerator.GetLabelDates(DateTime.Parse("1/1/07"), DateTime.Parse("1/3/07"), 5, 13);
			Assert.AreEqual(4, dates.Length);
			Assert.AreEqual(DateTime.Parse("1/1/07"), dates[0]);
			Assert.AreEqual(DateTime.Parse("1/2/07"), dates[1]);
			Assert.AreEqual(DateTime.Parse("1/3/07"), dates[2]);
			Assert.AreEqual(DateTime.Parse("1/4/07"), dates[3]);
		}

		[Test]
		public void GetLabelDates()
		{
			DateTime[] dates;

			dates = RankChartGenerator.GetLabelDates(DateTime.Parse("1/11/2007 9:02:57 PM"), DateTime.Parse("1/25/2007 5:10:53 PM"), 5, 10);
			Assert.AreEqual(6, dates.Length);
			Assert.AreEqual(DateTime.Parse("1/11/07"), dates[0]);
			Assert.AreEqual(DateTime.Parse("1/14/07"), dates[1]);
			Assert.AreEqual(DateTime.Parse("1/17/07"), dates[2]);
			Assert.AreEqual(DateTime.Parse("1/20/07"), dates[3]);
			Assert.AreEqual(DateTime.Parse("1/23/07"), dates[4]);
			Assert.AreEqual(DateTime.Parse("1/26/07"), dates[5]);
		}
	}
}
