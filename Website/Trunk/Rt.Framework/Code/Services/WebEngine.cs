using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;
using log4net;
using System.Reflection;

namespace Rt.Framework.Services
{
	public class WebEngine : RtEngineBase
	{
		public int _webServiceId;
		public int _serviceId;

		public WebEngine(Database db, int webServiceId, int serviceId)
			: base(db, false)
		{
			_webServiceId = webServiceId;
			_serviceId = serviceId;

			ReloadConfiguration();
		}

		public override int ServiceId
		{
			get { return _webServiceId; }
		}

		protected override void RunPreCycle()
		{
		}

		protected override void RunCycle()
		{
			Service service = _db.GetService(_serviceId);
			if (service.IsCrashSuspected())
			{
				try
				{
					RtEngineBase engine = RtEngines.GetRtEngine(_serviceId);

					if (engine != null)
						engine.RunFullCycle();
					else
						_log.DebugFormat("Engine {0} was null - It has not been initialized yet.", _serviceId);
				}
				finally
				{
					// Restore the service as we found it.
					_db.SaveService(service);
				}
			}
		}
	}
}
