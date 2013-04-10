package rt.javaTrayApplication.net;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.ProtocolException;
import java.net.URL;
import java.net.URLConnection;
import java.util.Iterator;
import java.util.Map;
import java.util.TreeMap;

public class HttpWebRequest {
	private URL _url;
	private URLConnection _urlConn;
	private HttpURLConnection _httpConn;
	private Map<String, Cookie> _cookieContainer;
	
	private HttpWebRequest(String url) throws IOException
	{
		_url = new URL(url);
		_urlConn = _url.openConnection();
		_httpConn = (HttpURLConnection)_urlConn;
	}
	
	public void setUserAgent(String value)
	{
		_urlConn.setRequestProperty("User-Agent", value);
	}
	
	public void setAllowAutoRedirect(boolean value)
	{
		HttpURLConnection.setFollowRedirects(value);
	}
	
	public void setMethod(String value) throws ProtocolException
	{
		_httpConn.setRequestMethod(value);
	}
	
	public void setContentType(String value)
	{
		_urlConn.setRequestProperty("Content-Type", value);
	}
	
	public void setContentLength(int value)
	{
		_urlConn.setRequestProperty("Content-Length", String.valueOf(value));
	}
	
	public void setCookieContainer(Map<String, Cookie> value)
	{
		_cookieContainer = value;
		attachCookies();
	}
	
	public static HttpWebRequest Create(String url) throws IOException
	{
		HttpWebRequest webRequest = new HttpWebRequest(url);
		return webRequest;
	}
	
	public OutputStream getRequestStream() throws IOException
	{
		_urlConn.setDoOutput(true);
		return _urlConn.getOutputStream();
	}
	
	public InputStream getResponseStream() throws IOException
	{
		InputStream input = _urlConn.getInputStream();
		saveCookies();
		return input;
	}
	
	public void setReadTimeout(int timeout)
	{
		_urlConn.setReadTimeout(timeout);
	}
	
	private void attachCookies()
	{
		StringBuilder sb = new StringBuilder();
		String domain = getDomain(_url);
		
		Iterator<String> keyIterator = _cookieContainer.keySet().iterator();
		while(keyIterator.hasNext())
		{
			String key = keyIterator.next();
			if( key.startsWith(domain + ":") || 
				key.startsWith("." + domain + ":"))
			{
				Cookie cookie = _cookieContainer.get(key);
				//if(cookie.getPath() == null || _url.getPath().startsWith(cookie.getPath()))
					sb.append(cookie.getName() + "=" + cookie.getContent() + "; ");
			}
		}

		if(sb.length() > 0)
		{
			// Set the cookie value to send
			System.out.println("Sending cookies: " + sb.toString());
	        _urlConn.setRequestProperty("Cookie", sb.toString());
		}
	}
	
	private void saveCookies()
	{
		if(_cookieContainer == null)
			_cookieContainer = new TreeMap<String, Cookie>();
		
		if(_urlConn.getHeaderFields() != null)
		{
			// Get all cookies from the server.
	        // Note: The first call to getHeaderFieldKey() will implicit send
	        // the HTTP request to the server.
	        for (int i=0; ; i++) {
	            String headerName = _urlConn.getHeaderFieldKey(i);
	            String headerValue = _urlConn.getHeaderField(i);
	            
	            if (headerName == null && headerValue == null) {
	                // No more headers
	                break;
	            }else if("Set-Cookie".equalsIgnoreCase(headerName)) {
	                // Parse cookie
	            	System.out.println("Recieved cookie: " + headerValue);
	            	Cookie cookie = Cookie.parse(headerValue, _url);
	            	_cookieContainer.put(cookie.getHost() + ":" + cookie.getName(), cookie);
	            }
	        }
		}
	}
	
	private static String getDomain(URL url)
	{
		String host = url.getHost();
		if(host.startsWith("www."))
    		return host.substring(4);
    	else
    		return host;
	}
}