using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Rt.Framework.Services;

namespace Rt.EmailEngine
{
	public partial class EmailEngine : ServiceBase
	{
		public EmailEngine()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
            RtEngines.EmailEngine.Start();          // Automatically starts the service.
		}

		protected override void OnStop()
		{
            RtEngines.EmailEngine.StopHeartbeat();    // Automatically stops the Service.
		}
	}
}
