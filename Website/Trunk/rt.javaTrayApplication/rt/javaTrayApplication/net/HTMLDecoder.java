package rt.javaTrayApplication.net;

public class HTMLDecoder {
	public static String decode(String html)
	{
		if(html == null)
			return null;
		
		String plainText = html;
		
		plainText = plainText.replace("&quot;", "\"");
		plainText = plainText.replace("&tab;", "\t");
		plainText = plainText.replace("&lt;", "<");
		plainText = plainText.replace("&gt;", ">");
		plainText = plainText.replace("&amp;", "&");
		
		return plainText;
	}
}
