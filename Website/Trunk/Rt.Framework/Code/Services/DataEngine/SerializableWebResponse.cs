using System;
using System.Collections.Generic;
using System.Text;

namespace Rt.Framework.Services.DataEngine
{
	public class SerializableWebResponse
	{
		private string _content;

		public string Content
		{
			get { return _content; }
			set { _content = value; }
		}
	}
}
