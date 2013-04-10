package rt.javaTrayApplication.net;

public class HTMLEncoder {
	public static String encode(String plainText)
	{
		if(plainText == null)
			return null;
		
		String html = plainText;
		
		html = html.replace("&", "&amp;");
		//html = html.replace("\t", "&tab;");
		html = html.replace("\"", "&quot;");
		html = html.replace("<", "&lt;");
		html = html.replace(">", "&gt;");
		
		return html;
	}
}
