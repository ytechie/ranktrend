using System;
using System.Web.UI;
using MichaelBrumm.Win32;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;

public partial class Administrators_WebService_Default : Page
{
	private const string PARAM_ID = "Id";

	private int? ServiceIdParameter
	{
		get
		{
			int id;
			object idParam = Request.QueryString[PARAM_ID];

			if (idParam == null) return null;

			if (int.TryParse(idParam.ToString(), out id))
				return id;
			else
				return null;
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initPage();
		}
	}

	private void initPage()
	{
		int? serviceId = ServiceIdParameter;
		ServiceName.Text = serviceId == null ? string.Empty : RtEngines.GetRtEngineName((int) ServiceIdParameter);
		loadEngineStatus();
	}

	private void loadEngineStatus()
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			Database db = Global.GetDbConnection();
			Service dbSvc = db.GetService(engine.ServiceId);
			DateTime serverTime = db.GetServerTime();

			Win32TimeZone timeZone = TimeZones.GetTimeZone(Profile.TimeZoneIndex);

			ServiceOwnerName.Text = engine.ServiceOwnerName;
			HeartBeating.Text = engine.IsHeartBeating ? string.Empty : "Not";
			IsRunning.Text = engine.IsRunning ? "Running" : "Stopped";
			IsServiceOwner.Text = engine.IsServiceOwner ? "Yes" : "No";
			WebServiceInformation.Style.Add("color",
			                                engine.IsServiceOwner && engine.IsRunning ||
			                                !engine.IsServiceOwner && !engine.IsRunning
			                                	? "green"
			                                	: "red");

			bool suspectedCrash =
				dbSvc.Enabled &&
				(Math.Abs(dbSvc.LastRunTime.Subtract(serverTime).TotalMinutes) > dbSvc.RunIntervalMinutes*2 ||
				 // More than 2 cycles times have passed since last run time
				 Math.Abs(dbSvc.LastHeartbeat.Subtract(serverTime).TotalMinutes) > EngineBase.HEARTBEAT_INTERVAL.TotalMinutes*2);
				// More than 2 heartbeat cycle times have passed since last heartbeat

			ServiceOwner.Text = dbSvc.Owner;
			LastRunTime.Text = dbSvc.LastRunTime == new DateTime() ? "Never" : timeZone.ToLocalTime(dbSvc.LastRunTime).ToString();
			LastHeartbeat.Text = dbSvc.LastHeartbeat == new DateTime()
			                     	? "Never"
			                     	: timeZone.ToLocalTime(dbSvc.LastHeartbeat).ToString();
			ServerTime.Text = timeZone.ToLocalTime(serverTime).ToString();
			NextRunTime.Text = dbSvc.Enabled
			                   	? "~" + timeZone.ToLocalTime(dbSvc.LastRunTime.AddMinutes(dbSvc.RunIntervalMinutes))
			                   	: "Next time service is started";

			EngineDatabaseStatus.Text = string.Format("{0}{1}{2}{3}",
			                                          suspectedCrash
			                                          	? "<span style=\"font-weight:bold;\">Suspected crash</span><br />"
			                                          	: string.Empty,
			                                          dbSvc.ForceRun ? "Forced run pending<br />" : string.Empty,
			                                          dbSvc.ReloadConfiguration
			                                          	? "Reload configuration pending<br />"
			                                          	: string.Empty,
			                                          dbSvc.ReloadConfiguration
			                                          	?
			                                          		(dbSvc.Enabled
			                                          		 	? "Will be started next heartbeat"
			                                          		 	: "Will be stopped next hearteat")
			                                          	:
			                                          		(dbSvc.Enabled ? "Enabled" : "Disabled"));
			ServiceInformation.Style.Add("color",
			                             suspectedCrash
			                             	? "red"
			                             	: dbSvc.Enabled && !dbSvc.ReloadConfiguration ? "green" : "#E0691A");

			StartHeartbeat.Visible = !engine.IsHeartBeating;
			StopHeartbeat.Visible = engine.IsHeartBeating;
			StartEngine.Visible = !engine.IsRunning;
			StopEngine.Visible = engine.IsRunning;
		}
		else
		{
			AjaxPanel1.Visible = false;
		}
	}

	#region Event Handlers

	protected void Refresh_Click(object sender, EventArgs e)
	{
		loadEngineStatus();
	}

	protected void StartHeartbeat_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.StartHeartbeat();
			loadEngineStatus();
		}
	}

	protected void StopHeartbeat_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.StopHeartbeat();
			loadEngineStatus();
		}
	}

	protected void StartEngine_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.Start();
			loadEngineStatus();
		}
	}

	protected void StopEngine_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.Stop();
			loadEngineStatus();
		}
	}

	protected void ForceEngineRun_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.RunFullCycle();
			loadEngineStatus();
		}
	}

	protected void ForceHeartbeat_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.ForceHeartbeat();
			loadEngineStatus();
		}
	}

	protected void ForceConfigReload_Click(object sender, EventArgs e)
	{
		int? serviceId = ServiceIdParameter;
		if (serviceId != null)
		{
			RtEngineBase engine = RtEngines.GetRtEngine((int) serviceId);
			engine.ForceReloadConfiguration();
			loadEngineStatus();
		}
	}

	#endregion
}