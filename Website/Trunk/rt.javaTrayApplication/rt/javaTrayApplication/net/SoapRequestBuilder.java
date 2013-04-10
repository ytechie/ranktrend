package rt.javaTrayApplication.net;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.Vector;

public class SoapRequestBuilder {
	public String Server = "";
	public String WebServicePath = "";
	public String SoapAction = "";
	public String MethodName = "";
	public String XmlNamespace = "";
	  
	public String LastError = null;
	  
	private Vector<Object> ParamNames = new Vector<Object>();
	private Vector<Object> ParamData = new Vector<Object>();
	
	public void AddParameter(String Name, String Data) {
		ParamNames.addElement( (Object) Name);
		ParamData.addElement( (Object) Data);
	}
	
	public String sendRequest() throws UnknownHostException, IOException, TimeoutException {
		String retval = "";
		Socket socket = null;
		socket = new Socket(Server, 80);
		
		boolean autoflush = true;
		PrintWriter out = new PrintWriter(socket.getOutputStream(), autoflush);
		//PrintStream out = System.out;
		BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream(), "UTF-8"));
		  
		String postData = getPostData();
		
		// send an HTTP request to the web service
		out.println("POST " + WebServicePath + " HTTP/1.1");
		out.println("Host: "+ Server);
		out.println("User-Agent: RankTrend Java Tray/" + getVersion() + " (Macintosh; U; Intel Mac OS X; en-US; rv:1.8.1.3)");
		out.println("Content-Type: text/xml; charset=utf-8");
		out.println("Content-Length: " + String.valueOf(postData.length()));
		out.println("SOAPAction: \"" + SoapAction + "\"");
		out.println("Connection: Close");
		out.println();
		out.println(postData);
		out.println();
		  
		// Read the response from the server ... times out if the response takes
		// more than 30 seconds
		String inputLine;
		StringBuffer sb = new StringBuffer(1000);
		
		int wait_seconds = 30;
		boolean timeout = false;
		long m = System.currentTimeMillis();
		while ( !in.ready() && !timeout) {
			if ( (System.currentTimeMillis() - m) > (1000 * wait_seconds)) timeout = true;
		}
		m = System.currentTimeMillis();
		if(!timeout)
		{
			while ( (inputLine = in.readLine()) != null && !timeout) {
				sb.append(inputLine + "\n");
				if ( (System.currentTimeMillis() - m) > (1000 * wait_seconds)) timeout = true;
			}
			in.close();
		}
		
		// The StringBuffer sb now contains the complete result from the
		// webservice in XML format.  You can parse this XML if you want to
		// get more complicated results than a single value.
		
		//System.out.println(sb.toString());
		
		if (!timeout) {
			String returnparam = MethodName + "Result";
		
			if(sb.toString().indexOf("<" + returnparam + ">") < 0)
				return null;
		
			int start = sb.toString().indexOf("<" + returnparam + ">") +
			returnparam.length() + 2;
			int end = sb.toString().indexOf("</" + returnparam + ">");
		
			//Extract a singe return parameter
			retval = sb.toString().substring(start, end);
		}
		else {
			throw new TimeoutException("Webservice response timed out");
		}
		
		socket.close();
		
		return retval;
	}
	  
	private String getPostData()
	{
		StringBuilder sb = new StringBuilder();
			  
		sb.append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
		sb.append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
		sb.append("<soap:Body>");
		sb.append("<" + MethodName + " xmlns=\"" + XmlNamespace + "\">");
		//Parameters passed to the method are added here
		for (int t = 0; t < ParamNames.size(); t++) {
			String name = (String) ParamNames.elementAt(t);
			String data = (String) ParamData.elementAt(t);
			sb.append("<" + name + ">" + data + "</" + name + ">");
		}
		sb.append("</" + MethodName + ">");
		sb.append("</soap:Body>");
		sb.append("</soap:Envelope>");
		      
		return sb.toString();
	}
	
	private static String getVersion()
	{
		Package p = SoapRequestBuilder.class.getPackage();
		if(p.getImplementationVersion() != null)
			return p.getImplementationVersion();
		else
			return "1.0.0.0";
	}
}