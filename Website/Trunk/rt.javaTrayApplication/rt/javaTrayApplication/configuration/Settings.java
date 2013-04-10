package rt.javaTrayApplication.configuration;


import java.beans.XMLDecoder;
import java.beans.XMLEncoder;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;

import org.apache.log4j.Logger;

import rt.io.Path;

public class Settings {
	private static Logger _log = Logger.getLogger(Settings.class);
	
	private static String _rtPath = null;
	
	private String _userName;
	private String _userGuid;
	
	public String getUserName()
	{
		return _userName;
	}
	
	public void setUserName(String value)
	{
		_userName = value;
	}
	
	public String getUserGuid()
	{
		return _userGuid;
	}
	
	public void setUserGuid(String value)
	{
		_userGuid = value;
	}
	
	public static Settings LoadSavedSettings()
	{
		Settings settings;
		
		File file = new File(getSettingsPath());
		if(file.exists())
		{
			FileInputStream os;
			try {
				os = new FileInputStream(getSettingsPath());
				XMLDecoder decoder = new XMLDecoder(os);
				settings = (Settings)decoder.readObject();
				decoder.close();
			} catch (FileNotFoundException e) {
				_log.error("An error occurred while attempting to load the settings.", e);
				return null;
			}
		}
		else
			settings = null;
		
		return settings;
	}
	
	public static void SaveSettings(Settings settings)
	{
		FileOutputStream os;
		try {
			os = new FileOutputStream(getSettingsPath(), true);
			
			XMLEncoder encoder = new XMLEncoder(os);
			encoder.writeObject(settings);
			encoder.close();
		} catch (FileNotFoundException e) {
			_log.error("An error occurred while attempting to save the settings.", e);
		}
	}
	
	public static String rtPath()
	{
		if(_rtPath == null)
		{
		String rtFolder = Path.combine("RankTrend", "TrayApplication");
		
		if(Path.library() == Path.home())
			rtFolder = "." + rtFolder;
		
		_rtPath = Path.combine(Path.library(), rtFolder);
		}
		
		return _rtPath;
	}
	
	private static String getSettingsPath()
	{
		File directory = new File(rtPath());
		if(!directory.exists())
			directory.mkdirs();
		
		return Path.combine(rtPath(), "Settings.xml");
	}
}
