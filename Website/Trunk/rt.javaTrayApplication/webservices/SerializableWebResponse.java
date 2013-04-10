package webservices;

import rt.javaTrayApplication.net.HTMLEncoder;

public class SerializableWebResponse {
	private String _content;
	
	public SerializableWebResponse(String content)
	{
		_content = content;
	}
	
	public String Serialize()
	{
		return "<Content>" + HTMLEncoder.encode(_content) + "</Content>";
	}
}
