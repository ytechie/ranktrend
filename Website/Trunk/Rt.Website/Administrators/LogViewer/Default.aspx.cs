using System;
using System.IO;
using System.Web.UI;
using Rt.Website;

public partial class Administrators_LogViewer_Default : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initPage();
		}
	}

	private void initPage()
	{
		string logPath, rLogPath;
		string log;

		logPath = Server.MapPath(Global.VirtualDirectory + "Logs/Log.txt");
		if (File.Exists(logPath))
		{
			DateTime timestamp = DateTime.Now;
			string myFileName = string.Format("Log{0}{1}.txt", timestamp.Minute, timestamp.Second);
			rLogPath = Path.Combine(Path.GetDirectoryName(logPath), myFileName);
			File.Copy(logPath, rLogPath, true);
			try
			{
				log = File.ReadAllText(rLogPath);
			}
			finally
			{
				File.Delete(rLogPath);
			}

			log = log.Replace(Environment.NewLine, "<br />");
			log = log.Replace("\n", "<br />");
			log = log.Replace("\r", string.Empty);
			litLog.Text = log;
		}
		else
		{
			litLog.Text = "Log does not exist.";
		}
	}
}