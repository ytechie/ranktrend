using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rt.Framework.Payments.PayPal
{
	[TestFixture]
	public class PayPalInterval_Tester
	{
		private PayPalInterval ppi;

		[Test]
		public void Parse()
		{
			ppi = new PayPalInterval("3 M");
			Assert.AreEqual(3, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Months, ppi.Units);
		}

		[Test]
		public void Parse2()
		{
			ppi = new PayPalInterval("5 D");
			Assert.AreEqual(5, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Days, ppi.Units);
		}

		[Test]
		public void Parse3()
		{
			ppi = new PayPalInterval("3 W");
			Assert.AreEqual(3, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Weeks, ppi.Units);
		}

		[Test]
		public void Parse4()
		{
			ppi = new PayPalInterval("3 Y");
			Assert.AreEqual(3, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Years, ppi.Units);
		}

		[Test]
		public void ctor()
		{
			ppi = new PayPalInterval(7, IntervalUnits.Weeks);
			Assert.AreEqual(7, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Weeks, ppi.Units);
		}

		[Test]
		public void AddTo()
		{
			DateTime now = DateTime.Now;

			ppi = new PayPalInterval(7, IntervalUnits.Weeks);

			Assert.AreEqual(now.AddDays(49), ppi.AddTo(now));
		}

		[Test]
		public void AddTo2()
		{
			DateTime now = DateTime.Now;

			ppi = new PayPalInterval(7, IntervalUnits.Days);

			Assert.AreEqual(now.AddDays(7), ppi.AddTo(now));
		}

		[Test]
		public void AddTo3()
		{
			DateTime now = DateTime.Now;

			ppi = new PayPalInterval(5, IntervalUnits.Months);

			Assert.AreEqual(now.AddMonths(5), ppi.AddTo(now));
		}

		[Test]
		public void AddTo4()
		{
			DateTime now = DateTime.Now;

			ppi = new PayPalInterval(6, IntervalUnits.Years);

			Assert.AreEqual(now.AddYears(6), ppi.AddTo(now));
		}

		[Test]
		public void Quantity()
		{
			ppi = new PayPalInterval("4 W");
			ppi.Quantity = 6;
			Assert.AreEqual(6, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Weeks, ppi.Units);
		}

		[Test]
		public void Units()
		{
			ppi = new PayPalInterval("4 W");
			ppi.Units = IntervalUnits.Months;
			Assert.AreEqual(4, ppi.Quantity);
			Assert.AreEqual(IntervalUnits.Months, ppi.Units);
		}

		[Test]
		public void TestToString()
		{
			ppi = new PayPalInterval("5 D");
			Assert.AreEqual("5 D", ppi.ToString());
		}

		[Test]
		public void TestToString2()
		{
			ppi = new PayPalInterval("5 W");
			Assert.AreEqual("5 W", ppi.ToString());
		}

		[Test]
		public void TestToString3()
		{
			ppi = new PayPalInterval("5 M");
			Assert.AreEqual("5 M", ppi.ToString());
		}

		[Test]
		public void TestToString4()
		{
			ppi = new PayPalInterval("5 Y");
			Assert.AreEqual("5 Y", ppi.ToString());
		}
	}
}
