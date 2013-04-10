using System;
using System.Web.UI;
using MichaelBrumm.Win32;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;

public partial class Administrators_ServiceControlPanelControl : UserControl
{
	public int ServiceId { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initControl();
		}
	}

	protected void StartEngine_Click(object sender, EventArgs e)
	{
		RtEngineBase engine = RtEngines.GetRtEngine(ServiceId);
		Database db = Global.GetDbConnection();
		Service dbSvc = db.GetService(engine.ServiceId);
		dbSvc.Enabled = true;
		dbSvc.ReloadConfiguration = true;
		db.SaveService(dbSvc);

		loadEngineStatus();
	}

	protected void StopEngine_Click(object sender, EventArgs e)
	{
		RtEngineBase engine = RtEngines.GetRtEngine(ServiceId);
		Database db = Global.GetDbConnection();
		Service dbSvc = db.GetService(engine.ServiceId);
		dbSvc.Enabled = false;
		dbSvc.ReloadConfiguration = true;
		db.SaveService(dbSvc);

		loadEngineStatus();
	}

	protected void ForceEngineRun_Click(object sender, EventArgs e)
	{
		RtEngineBase engine = RtEngines.GetRtEngine(ServiceId);
		Database db = Global.GetDbConnection();
		Service dbSvc = db.GetService(engine.ServiceId);
		dbSvc.ForceRun = true;
		db.SaveService(dbSvc);

		loadEngineStatus();
	}

	protected void ReloadEngineConfig_Click(object sender, EventArgs e)
	{
		RtEngineBase engine = RtEngines.GetRtEngine(ServiceId);
		Database db = Global.GetDbConnection();
		Service dbSvc = db.GetService(engine.ServiceId);
		dbSvc.ReloadConfiguration = true;
		db.SaveService(dbSvc);

		loadEngineStatus();
	}

	protected void ClearFlags_Click(object sender, EventArgs e)
	{
		RtEngineBase engine = RtEngines.GetRtEngine(ServiceId);
		Database db = Global.GetDbConnection();
		Service dbSvc = db.GetService(engine.ServiceId);
		dbSvc.ReloadConfiguration = false;
		dbSvc.ForceRun = false;
		db.SaveService(dbSvc);

		loadEngineStatus();
	}

	private void initControl()
	{
		WebServiceLink.NavigateUrl = string.Format("WebService/?Id={0}", ServiceId);
		WebServiceLink.Text = RtEngines.GetRtEngineName(ServiceId);
		loadEngineStatus();
	}

	private void loadEngineStatus()
	{
		Database db = Global.GetDbConnection();
		Service dbSvc = db.GetService(ServiceId);
		DateTime serverTime = db.GetServerTime();

		Win32TimeZone timeZone = TimeZones.GetTimeZone(Profile.TimeZoneIndex);

		bool suspectedCrash =
			dbSvc.Enabled &&
			(Math.Abs(dbSvc.LastRunTime.Subtract(serverTime).TotalMinutes) > dbSvc.RunIntervalMinutes*2 ||
			 // More than 2 cycles times have passed since last run time
			 Math.Abs(dbSvc.LastHeartbeat.Subtract(serverTime).TotalMinutes) > EngineBase.HEARTBEAT_INTERVAL.TotalMinutes*2);
			// More than 2 heartbeat cycle times have passed since last heartbeat

		Interval.Text = dbSvc.RunIntervalMinutes.ToString();
		LastRunTime.Text = dbSvc.LastRunTime == new DateTime() ? "Never" : timeZone.ToLocalTime(dbSvc.LastRunTime).ToString();
		LastHeartbeat.Text = dbSvc.LastHeartbeat == new DateTime()
		                     	? "Never"
		                     	: timeZone.ToLocalTime(dbSvc.LastHeartbeat).ToString();
		ServerTime.Text = timeZone.ToLocalTime(serverTime).ToString();
		NextRunTime.Text = dbSvc.Enabled
		                   	? "~" + timeZone.ToLocalTime(dbSvc.LastRunTime.AddMinutes(dbSvc.RunIntervalMinutes))
		                   	: "Next time service is started";

		EngineStatus.Text = string.Format("{0}{1}{2}{3}",
		                                  suspectedCrash
		                                  	? "<span style=\"font-weight:bold;\">Suspected crash (either 2 heartbeat or run times have past since last)</span><br />"
		                                  	: string.Empty,
		                                  dbSvc.ForceRun ? "Forced run pending<br />" : string.Empty,
		                                  dbSvc.ReloadConfiguration ? "Reload configuration pending<br />" : string.Empty,
		                                  dbSvc.ReloadConfiguration
		                                  	?
		                                  		(dbSvc.Enabled
		                                  		 	? "Will be started next heartbeat"
		                                  		 	: "Will be stopped next hearteat")
		                                  	:
		                                  		(dbSvc.Enabled ? "Enabled" : "Disabled"));
		StartEngine.Visible = !dbSvc.Enabled;
		StopEngine.Visible = dbSvc.Enabled;

		ServiceInformation.Style.Add("color",
		                             suspectedCrash
		                             	? "red"
		                             	: dbSvc.Enabled && !dbSvc.ReloadConfiguration ? "green" : "#E0691A");
	}
}