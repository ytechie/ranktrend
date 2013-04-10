using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Db.SqlServer;
using System.Threading;
using log4net;
using System.Reflection;

namespace Rt.Framework.Services
{
	/// <summary>
	///     This class is the class that all Service Engines can inherit.
	///     This class provides the heartbeat and cycle run logic that most
	///     services will be requiring.  Almost everything in this class
	///     is marked as <see cref="virtual" /> so that it can be overriden.
	/// </summary>
	public abstract class EngineBase
	{
		/// <summary>
		///     Defines how often the service should beat its heart.
		/// </summary>
		public static readonly TimeSpan HEARTBEAT_INTERVAL = TimeSpan.FromSeconds(60.0);

		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected int _cycleInterval;

		protected string _name;
		protected Timer _heartbeat;
		protected Timer _cycle;
		protected bool _isRunning;
		protected bool _isHeartBeating = false;
		protected int _heartbeatFailureCount = 0;
		protected int _cycleFailureCount = 0;
		protected bool _cycleExecuting = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cycleInterval">
		///     The amount of time the <see cref="EngineBase"/> should wait after completion of
		///     the previous cycle before running another cycle.  This can be changed at any time
		///     through the internal variable and will be picked up the next time the timer is set for
		///     the next cycle.
		/// </param>
		public EngineBase(string name, int cycleInterval)
		{
			_name = name;
			_cycleInterval = cycleInterval;

			_heartbeat = new Timer(new TimerCallback(HeartbeatCallback));
			_cycle = new Timer(new TimerCallback(CycleCallback));
		}

		/// <summary>
		///     Any actions that should be done before a cycle is run.
		/// </summary>
		protected abstract void RunPreCycle();
		/// <summary>
		///     The actions required to perform a "cycle".
		/// </summary>
		/// <remarks>
		///     If the RunPreCycle() fails, this method will not be executed.
		/// </remarks>
		protected abstract void RunCycle();
		/// <summary>
		///     Any actions that should be done after a cycle is run.
		/// </summary>
		/// <remarks>
		///     If the RunPreCycle() or RunCycle() fails, this method will not be executed.
		/// </remarks>
		protected abstract void RunPostCycle();
		/// <summary>
		///     The actions necessary for the service to send a "heartbeat" to 
		///     any monitoring devices to notify it that the service is still
		///     responding.
		/// </summary>
		protected abstract void Heartbeat();

		/// <summary>
		///     Gets whether or not the service is in a mode to send heartbeats.
		/// </summary>
		public bool IsHeartBeating
		{
			get { return _isHeartBeating; }
		}

		/// <summary>
		///		Gets whether or not the service is actually running.
		/// </summary>
		public virtual bool IsRunning
		{
			get { return _isRunning; }
		}

		/// <summary>
		///     Starts the service.  Also starts the service
		///     sending heartbeats if it has not already done so.
		/// </summary>
		/// <remarks>
		///     Automatically runs a full cycle.
		/// </remarks>
		public virtual void Start()
		{
			_log.InfoFormat("{0}: Starting Engine.", _name);
			StartHeartbeat();
			_isRunning = true;
			runFullCycle();
			SetCycle();
		}

		/// <summary>
		///     Stops the service.  The service will continue to 
		///     send heartbeats.
		/// </summary>
		public virtual void Stop()
		{
			_log.InfoFormat("{0}: Stopping Engine.", _name);
			_isRunning = false;
			SetCycle();
		}

		/// <summary>
		///     Performs a full cycle execution.  This function exists
		///     only for manual executions.  <see cref="runFullCycle"/>
		///     should be used internally.
		/// </summary>
		public virtual void RunFullCycle()
		{
			_log.InfoFormat("{0}: A full cycle execution was manually executed.", _name);
			runFullCycle();
			SetCycle();
		}

		/// <summary>
		///     Starts the service sending heartbeats.
		/// </summary>
		public virtual void StartHeartbeat()
		{
			_log.InfoFormat("{0}: Starting heart beating.", _name);
			_isHeartBeating = true;
			SetHeartbeat();
			Heartbeat();
		}

		/// <summary>
		///     Stops the service sending heartbeats.
		/// </summary>
		public virtual void StopHeartbeat()
		{
			_log.InfoFormat("{0}: Stopping heart beating.", _name);
			Stop();
			_isHeartBeating = false;
			SetHeartbeat();
		}

		/// <summary>
		///     Forces the service to send out a heartbeat.
		/// </summary>
		public virtual void ForceHeartbeat()
		{
			Heartbeat();
		}

		#region Event Handlers

		protected virtual void HeartbeatCallback(object state)
		{
			try
			{
				_log.DebugFormat("{0}: Heartbeat", _name);
				Heartbeat();
				_heartbeatFailureCount = 0;
			}
			catch (Exception ex)
			{
				_heartbeatFailureCount++;

				_log.Error(string.Format("{0}: An error has occurred during a heartbeat cycle.", _name), ex);

				if (_heartbeatFailureCount >= 5 && _heartbeatFailureCount % 5 == 0)
					_log.ErrorFormat("{0}: {1} successive errors have occurred during the heartbeat cycle.", _name, _heartbeatFailureCount);
			}
			finally
			{
				SetHeartbeat();
			}
		}

		protected virtual void CycleCallback(object state)
		{
			try
			{
				_log.DebugFormat("{0}: Cycle", _name);
				runFullCycle();
				_cycleFailureCount = 0;
			}
			catch (Exception ex)
			{
				_cycleFailureCount++;

				_log.Error(string.Format("{0}: An error has occurred during a cycle execution.", _name), ex);

				if (_cycleFailureCount >= 5 && _cycleFailureCount % 5 == 0)
					_log.ErrorFormat("{0}: {1} successive errors have occurred during the cycle execution.", _name, _heartbeatFailureCount);
			}
			finally
			{
				SetCycle();
			}
		}

		#endregion

		/// <summary>
		///     Sets the timer for the next heartbeat.
		/// </summary>
		protected void SetHeartbeat()
		{
			_log.DebugFormat("{0}: Resetting heartbeat timer.", _name);
			_heartbeat.Change(_isHeartBeating ? HEARTBEAT_INTERVAL : TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
		}

		/// <summary>
		///     Sets the timer for the next cycle.
		/// </summary>
		protected void SetCycle()
		{
			_log.DebugFormat("{0}: Resetting cycle timer.", _name);
			_cycle.Change(_isRunning ? _cycleInterval : Timeout.Infinite, Timeout.Infinite);
		}

		/// <summary>
		///     Runs a full cycle (Pre, Cycle, Post, Heartbeat).
		///     This is the function internal members should call.
		/// </summary>
		protected void runFullCycle()
		{
			if (_cycleExecuting)
			{
				_log.WarnFormat("{0}: Can not run a full cycle at this time.  Service is reporting that a cycle is already running.");
			}
			else
			{
				_cycleExecuting = true;
				try
				{
					_log.DebugFormat("{0}: Running full cycle.", _name);
					RunPreCycle();
					RunCycle();
					RunPostCycle();
					Heartbeat();
				}
				finally
				{
					_cycleExecuting = false;
				}
			}
		}
	}
}