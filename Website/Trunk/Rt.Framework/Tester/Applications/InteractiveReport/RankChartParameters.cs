using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rt.Framework.Applications.InteractiveReport
{
	[TestFixture]
	public class RankChartParameters_Tester
	{
		private RankChartParameters rcp;

		[SetUp]
		public void Setup()
		{
			rcp = new RankChartParameters();
		}

		[Test]
		public void StartTime()
		{
			DateTime referenceTime = DateTime.Now;

			rcp.StartTime = referenceTime;
			Assert.AreEqual(referenceTime, rcp.StartTime);
		}

		[Test]
		public void EndTime()
		{
			DateTime referenceTime = DateTime.Now;

			rcp.EndTime = referenceTime;
			Assert.AreEqual(referenceTime, rcp.EndTime);
		}

		#region Scrolling

		[Test]
		public void ScrollTimeRangeLeft()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 11:00");

			RankChartParameters.ScrollTimeRange(start, end, out start, out end, true);

			Assert.AreEqual(DateTime.Parse("1/1/07 9:15"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 10:15"), end);
		}

		[Test]
		public void ScrollTimeRangeLeft2()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 11:00");

			RankChartParameters.ScrollTimeRange(start, end, out start, out end, false);

			Assert.AreEqual(DateTime.Parse("1/1/07 10:45"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 11:45"), end);
		}

		[Test]
		public void ScrollTimeRange_NoSpan()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 10:00");

			RankChartParameters.ScrollTimeRange(start, end, out start, out end, false);

			Assert.AreEqual(DateTime.Parse("1/1/07 10:00"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 10:00"), end);
		}

		#endregion

		#region Zooming

		[Test]
		public void ZoomTimeRangeTest()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 11:00");

			RankChartParameters.ZoomTimeRange(start, end, out start, out end, true);

			Assert.AreEqual(DateTime.Parse("1/1/07 10:15"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 10:45"), end);
		}

		/// <summary>
		///		Verify that if the range can't be zoomed any more, that it stays the same.
		/// </summary>
		[Test]
		public void ZoomTimeRangeIn()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 10:00");

			RankChartParameters.ZoomTimeRange(start, end, out start, out end, true);

			Assert.AreEqual(DateTime.Parse("1/1/07 10:00"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 10:00"), end);
		}

		[Test]
		public void ZoomTimeRangeIn2()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 11:00");

			RankChartParameters.ZoomTimeRange(start, end, out start, out end, true);

			Assert.AreEqual(DateTime.Parse("1/1/07 10:15"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 10:45"), end);
		}

		/// <summary>
		///		Verify that if the range can't be zoomed any more, that it stays the same.
		/// </summary>
		[Test]
		public void ZoomTimeRangeOut()
		{
			DateTime start, end;

			start = DateTime.Parse("1/1/07 10:00");
			end = DateTime.Parse("1/1/07 11:00");

			RankChartParameters.ZoomTimeRange(start, end, out start, out end, false);

			Assert.AreEqual(DateTime.Parse("1/1/07 9:30"), start);
			Assert.AreEqual(DateTime.Parse("1/1/07 11:30"), end);
		}

		#endregion
	}
}
