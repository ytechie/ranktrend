package webservices;

import java.io.IOException;
import java.net.UnknownHostException;

import rt.javaTrayApplication.net.SoapRequestBuilder;
import rt.javaTrayApplication.net.TimeoutException;

public class TrayApplication {
	
	public String Authenticate(String userName, char[] password) throws UnknownHostException, IOException, TimeoutException
	{
		//System.out.println("Calling Authenticate from RankTrend TrayApplication Web Service.");
		SoapRequestBuilder s = getNewSoapRequest("Authenticate");
		
	    s.AddParameter("userName", userName);
	    s.AddParameter("password", String.valueOf(password));
	    
	    return s.sendRequest();
	}
	
	public String GetMinimumClientVersion() throws UnknownHostException, IOException, TimeoutException
	{
		//System.out.println("Calling GetMinimumClientVersion from RankTrend TrayApplication Web Service.");
		SoapRequestBuilder s = getNewSoapRequest("GetMinimumClientVersion");
		return s.sendRequest();
	}
	
	public String GetCurrentClientVersion() throws UnknownHostException, IOException, TimeoutException
	{
		//System.out.println("Calling GetCurrentClientVersion from RankTrend TrayApplication Web Service.");
		SoapRequestBuilder s = getNewSoapRequest("GetCurrentClientVersion");
		return s.sendRequest();
	}
	
	public String QueueNextDatasource(String guidString) throws UnknownHostException, IOException, TimeoutException
	{
		//System.out.println("Calling QueueNextDatasource from RankTrend TrayApplication Web Service.");
		SoapRequestBuilder s = getNewSoapRequest("QueueNextDatasource");
		
		s.AddParameter("guidString", guidString);
		
		return s.sendRequest();
	}
	
	public SerializableWebRequest GetDatasourceRequest(String requestKey) throws UnknownHostException, IOException, TimeoutException
	{
		//System.out.println("Calling GetDatasourceRequest from RankTrend TrayApplication Web Service.");
		String response;
		SoapRequestBuilder s = getNewSoapRequest("GetDatasourceRequest");
		
		s.AddParameter("requestKey", requestKey);
		
		response = s.sendRequest();
		return SerializableWebRequest.Deserialize(response);
	}
	
	public void SaveDatasourceResponse(SerializableWebResponse response, String requestKey) throws UnknownHostException, IOException, TimeoutException
	{
		//System.out.println("Calling SaveDatasourceResponse from RankTrend TrayApplication Web Service.");
		SoapRequestBuilder s = getNewSoapRequest("SaveDatasourceResponse");
		
		s.AddParameter("response", response.Serialize());
		s.AddParameter("requestKey", requestKey);
		
		s.sendRequest();
	}
	
	private SoapRequestBuilder getNewSoapRequest(String methodName)
	{
		SoapRequestBuilder s = new SoapRequestBuilder();

	    s.Server = "www.ranktrend.com";
	    s.MethodName = methodName;
	    s.XmlNamespace = "http://www.RankTrend.com/TrayAppplication";
	    s.WebServicePath = "/Webservices/TrayApplication.asmx";
	    s.SoapAction = s.XmlNamespace+"/"+s.MethodName;
	    
	    return s;
	}
}
