using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Rt.Framework.Services;
using System.ServiceProcess;
using log4net;
using System.Reflection;

namespace Rt.RssEventEngine
{
	public partial class RssEventEngine : ServiceBase
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public RssEventEngine()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_log.Info("Rank Trend RSS Event Engine Windows service is starting.");
			RtEngines.RssEventEngine.Start();          // Automatically starts the service.
		}

		protected override void OnStop()
		{
			_log.Info("Rank Trend RSS Event Engine Windows service is stopping.");
			RtEngines.RssEventEngine.StopHeartbeat();    // Automatically stops the Service.
		}
	}
}
