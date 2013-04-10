package rt.javaTrayApplication.net;

public class TimeoutException extends Exception {
	/**
	 * 
	 */
	private static final long serialVersionUID = 90365905445571840L;
	private String _message;
	
	public String getMessage()
	{
		return _message;
	}
	
	public TimeoutException(String message)
	{
		_message = message;
	}
}
