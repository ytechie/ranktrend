using System;
using Rt.Framework.Services;
using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	public class Service
	{
		private string _description;
		private bool _enabled;
		private bool _forceRun;
		private int? _id;
		private DateTime _lastHeartbeat;
		private DateTime _lastRunTime;
		private string _owner;
		private bool _reloadConfiguration;
		private int _runIntervalMinutes;
		private DateTime _serverTime;

		/// <summary>
		///     The unique ID of the service.
		/// </summary>
		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///     A short description of this service.
		/// </summary>
		[FieldMapping("Description")]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		///     The last time that the service "checked-in" to
		///     tell the database that it was working alright.
		/// </summary>
		[FieldMapping("LastHeartbeat")]
		public DateTime LastHeartbeat
		{
			get { return _lastHeartbeat; }
			set { _lastHeartbeat = value; }
		}

		/// <summary>
		///     If this is a polling service, the number of minutes
		///     between each sucessive working cycle.
		/// </summary>
		[FieldMapping("RunIntervalMinutes")]
		public int RunIntervalMinutes
		{
			get { return _runIntervalMinutes; }
			set { _runIntervalMinutes = value; }
		}

		/// <summary>
		///     Gets or sets whether or not the service should be doing
		///     work.
		/// </summary>
		/// <remarks>
		///     If a service is not enabled (enabled=false), then the service should
		///     look at this field, and wait for this value to change.
		/// </remarks>
		[FieldMapping("Enabled")]
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		/// <summary>
		///     Gets or sets the time that the service last ran.
		/// </summary>
		[FieldMapping("LastRunTime")]
		public DateTime LastRunTime
		{
			get { return _lastRunTime; }
			set { _lastRunTime = value; }
		}

		/// <summary>
		///     If true, the service will reload its configuration during the next heartbeat.
		/// </summary>
		[FieldMapping("ReloadConfiguration")]
		public Boolean ReloadConfiguration
		{
			get { return _reloadConfiguration; }
			set { _reloadConfiguration = value; }
		}

		/// <summary>
		///     If true, a cycle will be run during the next configuration load.
		/// </summary>
		/// <remarks>
		///     Configuration reloads typically happen during a heartbeat cycle.
		/// </remarks>
		[FieldMapping("ForceRun")]
		public Boolean ForceRun
		{
			get { return _forceRun; }
			set { _forceRun = value; }
		}

		/// <summary>
		///		Gets the time of the server when this object was loaded (so
		///		it can be compared with the last heartbeat and cycle run time).
		/// </summary>
		[FieldMapping("ServerTime")]
		public DateTime ServerTime
		{
			get { return _serverTime; }
			set { _serverTime = value; }
		}

		/// <summary>
		///		Gets or sets the owner of the service.
		/// </summary>
		[FieldMapping("Owner")]
		public string Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

		/// <summary>
		///		Returns whether or not the service is suspected to have
		///		crashed.  This is based on whether 2 cycle times have passed
		///		since the last run or 2 heartbeat cycle times have passed
		///		since the last heartbeat.
		/// </summary>
		/// <returns></returns>
		public bool IsCrashSuspected()
		{
			return
				Math.Abs(LastRunTime.Subtract(_serverTime).TotalMinutes) > RunIntervalMinutes*2 ||
				// More than 2 cycles times have passed since last run time
				Math.Abs(LastHeartbeat.Subtract(_serverTime).TotalMinutes) > EngineBase.HEARTBEAT_INTERVAL.TotalMinutes*2;
				// More than 2 heartbeat cycle times have passed since last heartbeat
		}
	}
}