using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rt.Framework.Components
{
	[TestFixture]
	public class ConfiguredDatasource_Tester
	{
		ConfiguredDatasource cd;

		[SetUp]
		public void Setup()
		{
			cd = new ConfiguredDatasource();
		}

		//[Test]
		//public void DatasourceTypeId()
		//{
		//  cd.DatasourceTypeId = 6;
		//  Assert.AreEqual(6, cd.DatasourceTypeId);
		//}

		[Test]
		public void CheckFrequencyDays()
		{
			cd.CheckFrequencyDays = 5;
			Assert.AreEqual(5, cd.CheckFrequencyDays);
			Assert.AreEqual(5, cd.CheckFrequency.TotalDays);
		}

		[Test]
		public void CheckFrequency()
		{
			cd.CheckFrequency = TimeSpan.FromDays(43.0);
			Assert.AreEqual(43, cd.CheckFrequencyDays);
			Assert.AreEqual(43, cd.CheckFrequency.TotalDays);
		}
	}
}
