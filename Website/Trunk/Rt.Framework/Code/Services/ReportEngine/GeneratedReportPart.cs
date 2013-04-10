using System;
using System.Collections.Generic;
using System.Text;

namespace Rt.Framework.Services.ReportEngine
{
	public class GeneratedReportPart
	{
		private string _html;
		private byte[] _bytes;

		public string Html
		{
			get { return _html; }
			set { _html = value; }
		}

		public byte[] Bytes
		{
			get { return _bytes; }
			set { _bytes = value; }
		}
	}
}
