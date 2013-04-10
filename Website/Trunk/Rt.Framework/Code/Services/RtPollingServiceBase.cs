//using System;
//using System.ServiceProcess;
//using System.Threading;
//using Microsoft.Win32;
//using Nle.Db;
//using Nle.Db.SqlServer;
//using Nle.LinkPage;
//using YTech.General;
//using System.IO;
//using log4net;
//using log4net.Config;
//using System.Reflection;
//using Nle.Components;

//namespace Rt.Services
//{
//  /// <summary>
//  ///		Provides a base for all of the services that use
//  ///		a pattern of polling.
//  /// </summary>
//  public abstract class RtPollingServiceBase : ServiceBase
//  {
//    private Timer _heartbeatTimer;
//    private Timer _pollTimer;
//    private Database _db;
//    private int _heartbeatFailureCount = 0;
//    private DateTime _nextPollTimerFire;

//    private static readonly TimeSpan HEARTBEAT_INTERVAL = TimeSpan.FromSeconds(60.0);

//    protected Service _serviceInfo;

//    private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//    public RtPollingServiceBase()
//    {
//      initLogging();
//    }

//    private void initLogging()
//    {
//      Uri uri;
//      string filePath;
//      FileInfo logConfig;

//      uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
//      filePath = Path.GetDirectoryName(uri.LocalPath);

//      logConfig = new FileInfo(filePath + System.IO.Path.DirectorySeparatorChar + "Logging.config");
//      XmlConfigurator.ConfigureAndWatch(logConfig);

//      _log.Info("Logging initialized");
//    }

//    /// <summary>
//    ///		Gets called when the service should perform the action
//    ///		that it needs to run.
//    /// </summary>
//    protected abstract void RunCycle();

//    #region Database Settings

//    protected abstract Database GetDatabase();
//    protected abstract int ServiceId
//    {
//      get;
//    }

//    #endregion

//    /// <summary>
//    ///		Set things in motion so your service can do its work.
//    /// </summary>
//    protected override void OnStart(string[] args)
//    {
//      _log.Debug("OnStart method called, calling base OnStart method");

//      base.OnStart(args);

//      _log.Debug("Retrieving database connection from inheriting service class");
//      _db = GetDatabase();

//      _log.Debug("Creating the timers");
//      _heartbeatTimer = new Timer(new TimerCallback(_heartbeatTimer_Tick));
//      _pollTimer = new Timer(new TimerCallback(_pollTimer_Tick));

//      _log.Info("Starting the heartbeat");
//      _heartbeatTimer_Tick(null);
//    }

//    private void runFullCycle()
//    {
//      try
//      {
//        if (_serviceInfo.Enabled)
//        {
//          _log.Info("Running the 'RunCycle' method of the inheriting service class");
//          RunCycle();
//          writeLastRunTime();
//        }
//        else
//        {
//          _log.Warn("The service is disabled, no work will be done during this cycle");
//        }
//      }
//      catch (Exception ex)
//      {
//        _log.Error("Critical error has occurred, but I'll keep letting the timer fire", ex);
//      }
//      finally
//      {
//        resetPollTimer();
//      }
//    }

//    private void resetPollTimer()
//    {
//      TimeSpan newTimerDuration;

//      //Determine the amount of time we need to wait to poll at the right time
//      _nextPollTimerFire = _serviceInfo.LastRunTime.AddMinutes(_serviceInfo.RunIntervalMinutes);
//      newTimerDuration = _nextPollTimerFire.Subtract(DateTime.UtcNow);

//      //If the time should fire now, or in the past, set the timespan accordingly
//      if (newTimerDuration.TotalMinutes <= 0)
//        newTimerDuration = TimeSpan.Zero;

//      _log.DebugFormat("Resetting poll timer to fire in {0} minutes", newTimerDuration.TotalMinutes);
//      _pollTimer.Change(newTimerDuration, TimeSpan.FromMilliseconds(-1));
//    }

//    /// <summary>
//    ///		Writes the last time that the service ran to the database.
//    /// </summary>
//    private void writeLastRunTime()
//    {
//      Database db;

//      db = GetDatabase();

//      _log.Debug("Writing the last runtime back to the registry");
//      //This could overwrite settings in the database!
//      _serviceInfo.LastRunTime = db.GetServerTime();
//      db.SaveService(_serviceInfo);
//    }

//    public void ReloadServiceConfiguration()
//    {
//      Service oldServiceInfo;
//      //use a new connection because we don't know if this is coming from
//      //the polling thread
//      Database db;

//      db = GetDatabase();

//      oldServiceInfo = _serviceInfo;
//      _serviceInfo = db.GetService(ServiceId);

//      //Check if we should run a cycle right now
//      if (_serviceInfo.ForceRun)
//      {
//        //Immediately stop the poll timer
//        _pollTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
//        _log.Info("The ForceRun flag was set to true, so the cycle is running immediately");

//        _log.Debug("Setting the ForceRun flag back to false, so the cycle doesn't get stuck on");
//        _serviceInfo.ForceRun = false;
//        _db.SaveService(_serviceInfo);

//        //Remember, this will reset the poll timer for us
//        runFullCycle();
//        return;
//      }

//      resetPollTimer();
//    }

//    private void _heartbeatTimer_Tick(object stateData)
//    {
//      bool ReloadConfiguration;

//      try
//      {
//        _log.Debug("Heartbeat timer has ticked");

//        _log.Debug("Saving heartbeat to the database");
//        _db.SaveServiceHeartbeat(ServiceId, out ReloadConfiguration);

//        if (_serviceInfo == null)
//        {
//          _log.Info("The service information hasn't been loaded, so it will be now");
//          ReloadConfiguration = true;
//        }

//        if (ReloadConfiguration)
//        {
//          _log.Info("Reload service configuration flag set");
//          ReloadServiceConfiguration();
//        }

//        if (_heartbeatFailureCount > 0)
//        {
//          _log.Info("Resetting hearbeat failure count");
//          _heartbeatFailureCount = 0;
//        }
//      }
//      catch (Exception ex)
//      {
//        _heartbeatFailureCount++;

//        _log.Debug("An error has occurred during a heartbeat cycle", ex);

//        if (_heartbeatFailureCount >= 5 && _heartbeatFailureCount % 5 == 0)
//          _log.Error(string.Format("{0} successive errors have occurred during the heartbeat cycle", _heartbeatFailureCount), ex);

//        _heartbeatFailureCount = 0;
//      }
//      finally
//      {
//        _log.Debug("Resetting heartbeat timer");
//        _heartbeatTimer.Change(HEARTBEAT_INTERVAL, TimeSpan.FromMilliseconds(-1));
//      }
//    }

//    private void _pollTimer_Tick(object stateData)
//    {
//      _log.Debug("Poll timer has ticked");
//      runFullCycle();
//    }
//  }
//}