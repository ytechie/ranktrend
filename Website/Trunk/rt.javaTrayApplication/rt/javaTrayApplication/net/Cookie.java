package rt.javaTrayApplication.net;

import java.net.URL;

public class Cookie {
	private String _name;
	private String _content;
	private String _host;
	private String _path;
	private String _expires;
	private boolean _secure;
	
	public String getName()
	{
		return _name;
	}
	
	public String getHost()
	{
		return _host;
	}
	
	public String getContent()
	{
		return _content;
	}
	
	public String getPath()
	{
		return _path;
	}
	
	public Cookie(String name, String content, String host, String path, String expires, boolean secure)
	{
		_name = name;
		_content = content;
		_host = host;
		_path = path;
		_expires = expires;
		_secure = secure;
	}
	
	public String toString()
	{
		StringBuilder sb = new StringBuilder();
		appendValue(sb, _name, _content);
		appendValue(sb, "expires", _expires);
		appendValue(sb, "domain", _host);
		appendValue(sb, "path", _path);
		
		if(_secure) sb.append("secure; ");
		
		return sb.toString();
	}
	
	public static Cookie parse(String cookieText, URL url)
	{
		// Parse cookie
        String[] fields = cookieText.split(";\\s*");

        String cookieValue = fields[0];
        String name = cookieValue.substring(0, cookieValue.indexOf("="));
        String content = cookieValue.substring(cookieValue.indexOf("=")+1);
        String expires = null;
        String path = null;
        String domain = null;
        boolean secure = false;

        // Parse each field
        for (int j=1; j<fields.length; j++) {
            if ("secure".equalsIgnoreCase(fields[j])) {
                secure = true;
            } else if (fields[j].indexOf('=') > 0) {
                String[] f = fields[j].split("=");
                if ("expires".equalsIgnoreCase(f[0])) {
                    expires = f[1];
                } else if ("domain".equalsIgnoreCase(f[0])) {
                    domain = f[1];
                } else if ("path".equalsIgnoreCase(f[0])) {
                    path = f[1];
                }
            }
        }
        
        if(domain == null) domain = getDomain(url);

        return new Cookie(name, content, domain, path, expires, secure);
	}
	
	private void appendValue(StringBuilder sb, String valueName, String value)
	{
		sb.append(valueName + "=" + value + "; ");
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
