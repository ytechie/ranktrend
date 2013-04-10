using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rt.TrayApplication
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool grantedOwnership;
			using (System.Threading.Mutex mtxSingleInstance = new System.Threading.Mutex(true, "93803d66-bd68-4d22-8e8b-93fb37e0d05d", out grantedOwnership))
			{
				if (grantedOwnership)
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new MainApplicationContext());
				}
				else
					MainApplicationContext.ShowMessageBox("Another instance of the RankTrend Tray Application is already running.  By default, this application is configured to run on Startup and you should not have to manually launch it.");
			}
		}
	}
}