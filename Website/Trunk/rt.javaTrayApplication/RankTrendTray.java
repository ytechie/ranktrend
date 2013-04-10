

import java.io.File;

import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

import org.apache.log4j.Logger;
import org.apache.log4j.helpers.Loader;
import org.apache.log4j.xml.DOMConfigurator;

import rt.javaTrayApplication.configuration.Settings;
import rt.io.Path;
import rt.javaTrayApplication.MainApplicationContext;

/**
 * @author shawnriesterer
 *
 */
public class RankTrendTray {
	private static Logger _log = Logger.getLogger(RankTrendTray.class);
	
	private static String LOGFILENAME = "logging.config";
	
	/**
	 * @param args
	 */
	public static void main(String[] args) {
		try {
			setSystemProperties();
			loadLoggingConfig();
		}
		catch (Exception e) {
			e.printStackTrace();
			System.exit(0);
		}
		
		try {
			setNativeLookAndFeel();
			
			new MainApplicationContext();
		} catch (Exception e) {
			_log.fatal(e);
			System.exit(0);
		}
	}
	
	private static void setSystemProperties()
	{
		System.setProperty("user.lib", Settings.rtPath());
		System.setProperty("apple.laf.useScreenMenuBar", "true");
	}
	
	private static void setNativeLookAndFeel() throws ClassNotFoundException, InstantiationException, IllegalAccessException, UnsupportedLookAndFeelException
    {
		UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
    }
	
	private static void loadLoggingConfig()
	{
		String userLoggingConfig = Path.combine(Settings.rtPath(), LOGFILENAME);
		File loggingConfig = new File(userLoggingConfig);
		
		if(loggingConfig.exists())
			DOMConfigurator.configure(userLoggingConfig);
		else
			DOMConfigurator.configure(Loader.getResource("logging.config"));
	}
}
