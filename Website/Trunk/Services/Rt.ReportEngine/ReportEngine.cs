using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Rt.Framework.Services;
using System.ServiceProcess;
using log4net;
using System.Reflection;

namespace Rt.ReportEngine
{
	public partial class ReportEngine : ServiceBase
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public ReportEngine()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_log.Info("RankTrend Report Engine Windows service is starting.");
			RtEngines.ReportEngine.Start();          // Automatically starts the service.
		}

		protected override void OnStop()
		{
			_log.Info("RankTrend Report Engine Windows service is stopping.");
			RtEngines.ReportEngine.StopHeartbeat();    // Automatically stops the Service.
		}
	}
}
