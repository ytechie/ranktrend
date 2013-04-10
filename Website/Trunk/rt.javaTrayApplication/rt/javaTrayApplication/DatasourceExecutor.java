package rt.javaTrayApplication;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.UnknownHostException;
import java.util.Map;
import java.util.Random;
import java.util.TreeMap;

import org.apache.log4j.Logger;

import rt.javaTrayApplication.net.Cookie;
import rt.javaTrayApplication.net.HttpWebRequest;
import rt.javaTrayApplication.net.TimeoutException;

import webservices.SerializableWebRequest;
import webservices.SerializableWebResponse;

public class DatasourceExecutor {
	private static Logger _log = Logger.getLogger(DatasourceExecutor.class);
	
	// Use the same cookies for all requests/responses
	private Map<String, Cookie> _cookies;

	private String _guidString;

	public DatasourceExecutor(String guidString)
	{
		_guidString = guidString;
	}

	public void ExecutePendingJobs() throws UnknownHostException, IOException, TimeoutException
	{
		webservices.TrayApplication ws;
		String requestKey;
		SerializableWebRequest currentRequest;
		InputStream httpResponse;
		SerializableWebResponse response;
		Random generator = new Random();

		ws = new webservices.TrayApplication();
		
		for (requestKey = ws.QueueNextDatasource(_guidString);
		     requestKey != null;
		     requestKey = ws.QueueNextDatasource(_guidString))
		{	
			_log.debug("A request was queued as key '" + requestKey + "'");

			_cookies = new TreeMap<String, Cookie>();

			for (currentRequest = ws.GetDatasourceRequest(requestKey);
			     currentRequest != null;
			     currentRequest = ws.GetDatasourceRequest(requestKey))
			{	
				_log.debug("Executing a request for request key '" + requestKey + "'");

				httpResponse = ProcessRequest(currentRequest);
				response = WrapHttpResponse(httpResponse);

				_log.info("Saving the response for request key '" + requestKey + "'");
				ws.SaveDatasourceResponse(response, requestKey);
			}
			
			// Random delay between 30 seconds and 3 minutes
			int delay = generator.nextInt(2 * 60 * 1000 + 30000) + 30000;		// Random number 0:00 - 2:30
			_log.debug("Waiting " + delay + "ms before getting next pending datasource task.");
			try {
				Thread.sleep(delay);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				_log.warn("Artificial delay between pending datasource tasks was interrupted.", e);
				e.printStackTrace();
			}
		}
	}
	
	public InputStream ProcessRequest(SerializableWebRequest request) throws IOException
	{
		HttpWebRequest httpRequest;
		byte[] postBytes;
		OutputStream requestStream;
		
		httpRequest = HttpWebRequest.Create(request.getUrl());
		httpRequest.setCookieContainer(_cookies);
		// Todo: randomize this
		httpRequest.setUserAgent("Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1");
		// We need this, particularly for Google login stuff
		httpRequest.setAllowAutoRedirect(true);
		
		// Check if there is any data to post
		if (request.getPostData() != null)
		{
			//Set up the post data
			postBytes = request.getPostData().getBytes("UTF-8");
			
			httpRequest.setMethod("POST");
			httpRequest.setContentType("application/x-www-form-urlencoded");
			httpRequest.setContentLength(postBytes.length);
			requestStream = httpRequest.getRequestStream();
			requestStream.write(postBytes, 0, postBytes.length);
			requestStream.flush();
			requestStream.close();
		}
		
		return httpRequest.getResponseStream();
	}
	
	private SerializableWebResponse WrapHttpResponse(InputStream response) throws IOException
	{
		StringBuilder sb = new StringBuilder();
        boolean utf16 = false;
	    int c;
	    
	    while ((c = response.read()) != -1) {
	    	if(c == 255)
	    		utf16 = true;
	    	if(!(utf16 && (c == 0 || c== 255 || c == 254)))
	    		sb.append((char)c);
	    }
	    
	    return new SerializableWebResponse(sb.toString());
	}
}
