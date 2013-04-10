package rt.javaTrayApplication;

import java.util.Timer;
import java.util.TimerTask;

import org.apache.log4j.Logger;

import rt.javaTrayApplication.configuration.Settings;

public class Runner extends TimerTask {
	private static Logger _log = Logger.getLogger(Runner.class);
	
	private final int DELAY = 0 * 60 * 1000;			// Delay the first execution 3 minutes to allow net. conn.
	private final int PERIOD = 1 * 60 * 60 * 1000;		// 1 hour between cycles
	
	private Settings _settings;
	private Timer _pollingTimer;
	
	public Runner(Settings settings)
	{
		_settings = settings;

		_log.debug("Starting up, queueing a user work item to run a cycle.");
		startTimer();
	}
	
	public void run() {
		try
		{
			DatasourceExecutor de;

			if (_settings.getUserGuid() == null)
			{
				_log.info("The user does not have a GUID, which means they have not yet entered their credentials");
				System.out.println("The user does not have a GUID, which means they have not yet entered their credentials");
				return;
			}

			_log.debug("Polling for pending jobs");

			de = new DatasourceExecutor(_settings.getUserGuid());
			de.ExecutePendingJobs();
			
			_log.debug("No more pending jobs.");
		}
		catch (Exception e)
		{
			_log.warn("There was a problem running a cycle.  This could simply indicate a temporary connection issues with the server", e);
		}
	}
	
	private void startTimer()
	{	
		if (_pollingTimer == null)
		{
			int delayMinutes = DELAY / 60 / 1000;
			_log.debug("Creating a new polling timer but delaying its first execution for " + delayMinutes + " minutes.");
			_pollingTimer = new Timer(true);
			_pollingTimer.schedule(this, DELAY,  PERIOD);
		}
	}
}
