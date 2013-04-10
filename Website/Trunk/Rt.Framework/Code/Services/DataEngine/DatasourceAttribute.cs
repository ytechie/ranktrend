using System;
using System.Collections.Generic;
using System.Text;

namespace Rt.Framework.Services.DataEngine
{
	[AttributeUsage(AttributeTargets.Class)] 
	public class DatasourceAttribute : Attribute
	{
		private int _uniqueId;

		public DatasourceAttribute(int uniqueId)
		{
			_uniqueId = uniqueId;
		}

		public int UniqueId
		{
			get { return _uniqueId; }
		}
	}
}
