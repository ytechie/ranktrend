package rt.io;

import java.io.File;

public class Path
{
	private static String _home = null;
	private static String _library = null;
	private static String separator = System.getProperty("file.separator");
	
	public static String home()
	{
		if(_home == null)
			_home = System.getProperty("user.home");
		
		return _home;
	}
	
	public static String library()
	{
		if(_library == null)
		{
			String windowsPath = System.getenv("APPDATA");
			String macPath = combine(home(), "Library");
			
			if (macPath != null && new File(macPath).exists())
				return macPath;
			else if(windowsPath != null && new File(windowsPath).exists())
				return windowsPath;
			else
				return home();
		}
		
		return _library;
	}
	
	public static String combine(String path1, String path2)
	{
		if(path1.substring(path1.length() - 1) == separator)
			return path1 + path2;
		else
			return path1 + separator + path2;
	}
}
