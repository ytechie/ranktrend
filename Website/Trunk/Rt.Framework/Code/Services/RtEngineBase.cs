using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;
using System.Configuration;

namespace Rt.Framework.Services
{
    /// <summary>
    ///     This class is the class that all RankTrend Service Engines can inherit.
    ///     This class inherently provides the heartbeat and cycle run logic that most
    ///     services will be requiring.  Almost everything in this class
    ///     is overridable.
    /// </summary>
	public abstract class RtEngineBase : EngineBase
	{
		private const string APPSETTING_SERVICEOWNERNAME = "RT_ServiceOwnerName";

		protected Database _db;
		protected Service _service;
		protected string _serviceOwnerName;

		public RtEngineBase(Database db)
			: base("RtEngineBase", 60 * 1000)
		{
			ctor(db, true);
		}

		public RtEngineBase(Database db, bool reloadConfiguration)
			: base("RtEngineBase", 60 * 1000)
		{
			ctor(db, reloadConfiguration);
		}

		private void ctor(Database db, bool reloadConfiguration)
		{
			_serviceOwnerName = ConfigurationManager.AppSettings[APPSETTING_SERVICEOWNERNAME];
			if (_serviceOwnerName == null) _serviceOwnerName = "Windows";

			_db = db;
			if(reloadConfiguration) ReloadConfiguration();
		}

		/// <summary>
		///     The Id of this service.
		/// </summary>
		public abstract int ServiceId { get; }

		public bool IsEnabled
		{
			get { return _service.Enabled; }
		}

		public override bool IsRunning
		{
			get { return base.IsRunning; }
		}

		public DateTime LastHeartbeat
		{
			get { return _service.LastHeartbeat; }
		}

		public DateTime LastRunTime
		{
			get { return _service.LastRunTime; }
		}

		public int RunIntervalMinutes
		{
			get { return _service.RunIntervalMinutes; }
		}

		public string ServiceOwner
		{
			get { return _service.Owner; }
		}

		public bool IsServiceOwner
		{
			get { return ServiceOwner == _serviceOwnerName; }
		}

		public string ServiceOwnerName
		{
			get { return _serviceOwnerName; }
		}

		/// <summary>
		///     Any actions that should be done after a cycle is run.
		/// </summary>
		/// <remarks>
		///     If the RunPreCycle() or RunCycle() fails, this method will not be executed.
		/// </remarks>
		protected override void RunPostCycle()
		{
			bool shouldReload = false, forceRun = false;
			SaveRunTime(out shouldReload, out forceRun);
			if (shouldReload) ReloadConfiguration();
			if (forceRun) runFullCycle();
		}

		/// <summary>
		///     The actions necessary for the service to send a "heartbeat" to 
		///     any monitoring devices to notify it that the service is still
		///     responding.
		/// </summary>
		protected override void Heartbeat()
		{
			bool shouldReload = false, forceRun = false;
			SaveHeartbeat(out shouldReload, out forceRun);
			if (shouldReload) ReloadConfiguration();
			if (forceRun) runFullCycle();
		}

		/// <summary>
		///     Starts the service.  Also starts the service
		///     sending heartbeats if it has not already done so.
		/// </summary>
		/// <remarks>
		///     Overridden here to save information about the service's
		///     running state to the database.
		/// </remarks>
		public override void Start()
		{
			_service = _db.GetService(ServiceId);

			if (IsServiceOwner)
			{
				_service.Enabled = true;
				_db.SaveService(_service);

				base.Start();
			}
			else
			{
				_log.InfoFormat("{0}: {1} is not allowed to start this engine.  {2} is configured as the owner of this service.  Starting the heartbeat but stopping this engine.", _name, _serviceOwnerName, ServiceOwner);
				base.Stop();
				StartHeartbeat();
			}
		}

		/// <summary>
		///     Stops the service.  The service will continue to 
		///     send heartbeats.
		/// </summary>
		/// <remarks>
		///     Overridden here to save information about the service's
		///     running state to the database.
		/// </remarks>
		public override void Stop()
		{
			base.Stop();

			_service = _db.GetService(ServiceId);
			if (IsServiceOwner)
			{
				_service.Enabled = false;
				_db.SaveService(_service);
			}
			else
			{
				_log.InfoFormat("{0}: {1} is not allowed to disable this engine.  {2} is configured as the owner of this service.", _name, _serviceOwnerName, ServiceOwner);
			}
		}

		/// <summary>
		///		Forces an immediate reload of the configuration.
		/// </summary>
		public virtual void ForceReloadConfiguration()
		{
			ReloadConfiguration();
		}

		/// <summary>
		///     Reloads the configuration from the database.
		///     This includes the service information and resets
		///     the cycle time interval on the base class.
		/// </summary>
		protected virtual void ReloadConfiguration()
		{
			_log.DebugFormat("{0}: Reloading configuration.", _name);
			_service = _db.GetService(ServiceId);
			base._name = _service.Description;
			base._cycleInterval = _service.RunIntervalMinutes * 60 * 1000;
			if (IsServiceOwner)
			{
				if (_service.Enabled && !IsRunning)
					Start();
				else if (!_service.Enabled && IsRunning)
					Stop();
			}
			else
			{
				if(!IsHeartBeating) StartHeartbeat();
				if(_isRunning)		// Have to check field, not property
				{
					_log.InfoFormat("{0}: {1} is no longer configured as the owner of this service, {2} is.  Stopping this engine.", _name, _serviceOwnerName, ServiceOwner);
					base.Stop();
				}
			}
		}

		protected virtual void SaveHeartbeat(out bool shouldReload, out bool forceRun)
		{
			if (IsServiceOwner)
			{
				if (_db != null)
				{
					_log.DebugFormat("{0}: Saving heartbeat to database.", _name);
					_db.SaveServiceHeartbeat(ServiceId, out shouldReload, out forceRun);
				}
				else
				{
					shouldReload = false;
					forceRun = false;
				}
			}
			else
			{
				_log.DebugFormat("{0}: {1} is not configured as the owner of this service, {2} is.  Not committing heartbeat to the database.", _name, _serviceOwnerName, ServiceOwner);
				shouldReload = false;
				forceRun = false;
			}
		}

		protected virtual void SaveRunTime(out bool shouldReload, out bool forceRun)
		{
			if (_db != null)
			{
				_log.DebugFormat("{0}: Saving run time to database.", _name);
				_db.SaveServiceRunTime(ServiceId, out shouldReload, out forceRun);
			}
			else
			{
				shouldReload = false;
				forceRun = false;
			}
		}
	}
}
