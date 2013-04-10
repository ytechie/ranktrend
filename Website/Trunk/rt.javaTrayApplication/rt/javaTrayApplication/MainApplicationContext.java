package rt.javaTrayApplication;

import java.io.IOException;
import java.net.UnknownHostException;

import javax.swing.JOptionPane;

import org.apache.log4j.Logger;

import rt.javaTrayApplication.configuration.Settings;
import rt.javaTrayApplication.net.TimeoutException;

public class MainApplicationContext {
	private static Logger _log = Logger.getLogger(MainApplicationContext.class);
	
	public static String APP_NAME = "RankTrend Tray";
	
	private static MainForm settingsForm;
	private static Settings _settings;
	private static Runner _runner;
	
	public MainApplicationContext() throws UnknownHostException, IOException, TimeoutException
	{
		_log.info("Starting main application context");
		
		if(isMacOs())
		{
			MacApp macApp = new MacApp();
			macApp.Configure();
		}
		
		if(settingsForm == null)
		{
			_log.debug("Creating a new instance of the settings form");
			settingsForm = new MainForm();
		}
		
		checkVersion();
		loadSettings();
		
		if(_runner == null)
			_runner = new Runner(getSettings());
	}
	
	public static Settings getSettings()
	{
		return _settings;
	}
	
	public static void setSettings(Settings value)
	{
		_settings = value;
	}
	
	public static void ShowMessageBox(String message)
	{
		JOptionPane.showMessageDialog(null, message);
	}
	
	private static boolean isMacOs()
	{
		return System.getProperties().getProperty( "os.name" ).indexOf("Mac") >= 0;
	}
	
	private static String getVersion()
	{
		Package p = MainApplicationContext.class.getPackage();
		if(p.getImplementationVersion() != null)
			return p.getImplementationVersion();
		else
			return "1.0.0.0";
	}
	
	private void loadSettings()
	{ 
		_settings = Settings.LoadSavedSettings();

		if (getSettings() == null)
		{
			MainApplicationContext.ShowMessageBox("You need to enter your credentials.");

			//We need the main form to create the initial options
			settingsForm.Show();
		}
	}
	
	private void checkVersion() throws UnknownHostException, IOException, TimeoutException
	{
		Version thisVersion = new Version(getVersion());
		
		webservices.TrayApplication ws;
		String minVersionString;
		Version minVersion;

		ws = new webservices.TrayApplication();
		minVersionString = ws.GetMinimumClientVersion();
		minVersion = new Version(minVersionString);
			
		if (minVersion.CompareTo(thisVersion) > 0)
		{
			MainApplicationContext.ShowMessageBox("The version of the RankTrend.com tray application that you are using is out of date, and must be upgraded, you will now be taken to the upgrade page.");
			launchRankTrend("Members/Tray-Application/");
			System.exit(0);
		}
	}
	
	private void launchRankTrend(String subPage)
	{
		try {
			String url = "http://www.RankTrend.com/";
			
			if(subPage != null)
				url += subPage;
			
			new ProcessBuilder("open", url).start();
		} catch (IOException e) {
			_log.warn("Could not launch RankTrend.com.", e);
		}
	}
	
	private class Version
	{
		public final int GREATERTHAN = 1;
		public final int LESSTHAN = -1;
		public final int EQUALTO = 0;
		
		private String _versionString; 
		private String[] _version;
		
		public Version(String version)
		{
			if(version != null)
			{
				_versionString = version;
				_version = version.split("\\.");
			}
		}
		
		public String toString()
		{
			return _versionString;
		}
		
		public int CompareTo(Version compareToVersion)
		{
			int ubound;
			
			if(_version == null && compareToVersion._version != null)
				return LESSTHAN;
			else if(_version == null && compareToVersion._version == null)
				return EQUALTO;
			else if(_version != null && compareToVersion._version == null)
				return GREATERTHAN;
			
			// Find the largest index that the two versions have in common
			// e.g. 1.0 vs. 1.0.0.0, 2 would be the highest index
			if(_version.length < compareToVersion._version.length)
				ubound = _version.length;
			else
				ubound = compareToVersion._version.length;
			
			// Compare each part of the versions
			// 1.1 is higher than 1.0, 1.2.3 is higher than 1.2.2
			for(int i = 0; i < ubound; i++)
			{
				int myNum, ctNum;
				
				myNum = Integer.parseInt(_version[i]);
				ctNum = Integer.parseInt(compareToVersion._version[i]);
			
				if(myNum < ctNum)
					return LESSTHAN;
				else if(myNum > ctNum)
					return GREATERTHAN;
			}
			
			// If one version string has more parts to it, than it is a newer version
			// (for simplicity, even if those extra parts are 0's, it is still newer)
			// e.g. 1.0.0.0 is greater than 1.0 and so is 1.0.1 vs 1.0
			if(_version.length < compareToVersion._version.length)
				return LESSTHAN;
			else if(_version.length > compareToVersion._version.length)
				return GREATERTHAN;
			
			return EQUALTO;
		}
	}
}
