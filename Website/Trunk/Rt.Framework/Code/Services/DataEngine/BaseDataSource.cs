using System;
using System.Collections.Generic;
using System.Text;

namespace Rt.Framework.Services.DataEngine
{
	public class BaseDataSource
	{
		protected Dictionary<int, object> _parameters;
		protected string _url;
		
		public Dictionary<int, object> Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}
	}
}
