package webservices;

import rt.javaTrayApplication.net.HTMLDecoder;

public class SerializableWebRequest {
	private String _url;
	private String _postData;
	
	private SerializableWebRequest() {}
	
	public String getUrl()
	{
		return _url;
	}
	
	public String getPostData()
	{
		return _postData;
	}
	
	public static SerializableWebRequest Deserialize(String serializedWebRequest)
	{
		SerializableWebRequest swr = new SerializableWebRequest();
		
		if(serializedWebRequest != null)
		{
			swr._url = HTMLDecoder.decode(getValue(serializedWebRequest, "Url")).replace(" ", "%20");
			swr._postData = HTMLDecoder.decode(getValue(serializedWebRequest, "PostData"));
			
			return swr;
		}
		else
			return null;
	}
	
	private static String getValue(String serializedString, String propertyName)
	{        
        if(serializedString.indexOf("<" + propertyName + ">") < 0)
        	return null;
        
        int start = serializedString.indexOf("<" + propertyName + ">") +
        propertyName.length() + 2;
        int end = serializedString.indexOf("</" + propertyName + ">");

        return serializedString.substring(start, end);
	}
}
