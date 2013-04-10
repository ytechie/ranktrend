using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Rt.TrayApplication.Configuration;
using log4net;
using System.Reflection;

namespace Rt.TrayApplication
{
	class Runner : IDisposable
	{
		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private Settings _settings;
		private Timer _pollingTimer;
		private IStatus _status;

		public Runner(Settings settings, IStatus status)
		{
			_settings = settings;
			_status = status;
			try
			{
				_log.Debug("Starting up, queueing a user work item to run a cycle.");
				ThreadPool.QueueUserWorkItem(new WaitCallback(runCycleCallback));
			}
			finally
			{
				startTimer();
			}
		}

		private void startTimer()
		{
			TimeSpan pollInterval;

			if (_pollingTimer == null)
			{
				_log.Debug("Creating a new polling timer");
				_pollingTimer = new Timer(new TimerCallback(timerCallback));
			}

			pollInterval = TimeSpan.FromHours(_settings.PollHours);

			_log.DebugFormat("Setting the polling timer to fire every {0} hours, next poll time is appromately {1}",
				_settings.PollHours, DateTime.Now.Add(pollInterval));
			_pollingTimer.Change(pollInterval, TimeSpan.FromMilliseconds(-1));

		}

		private void timerCallback(object state)
		{
			try
			{
				_log.Debug("Timer fired, running the cycle");
				_status.Status = "Communicating with server";
				RunCycle();
			}
			finally
			{
				_log.Debug("Restarting timer");
				//Start the timer again
				startTimer();
				_status.Status = "Idle";
			}
		}

		public void RunCycle()
		{
			runCycleCallback(null);
		}

		/// <summary>
		///		Contacts the server and runs any pending jobs.
		/// </summary>
		private void runCycleCallback(object state)
		{
			try
			{
				DatasourceExecutor de;

				if (_settings.UserGuid == new Guid())
				{
					_log.Info("The user does not have a GUID, which means they have not yet entered their credentials");
					return;
				}

				_log.Debug("Polling for pending jobs");

				de = new DatasourceExecutor(_settings.UserGuid.ToString());
				de.ExecutePendingJobs();
			}
			catch (Exception ex)
			{
				_log.Warn("There was a problem running a cycle.  This could simply indicate a temporary connection issues with the server", ex);
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				_pollingTimer.Dispose();
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
