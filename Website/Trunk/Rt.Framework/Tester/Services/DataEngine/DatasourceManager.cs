using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rt.Framework.Services.DataEngine
{
	[TestFixture]
	public class DatasourceManager_Tester
	{
		[Test]
		public void Test()
		{
			DatasourceManager dm;
			IDataSource ds;

			dm = new DatasourceManager();
			ds = dm.GetDatasource(1);
		}
	}
}
