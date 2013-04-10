using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using log4net;
using Rt.TrayApplication.Webservices;
using System;
using System.Threading;

namespace Rt.TrayApplication
{
	public class DatasourceExecutor
	{
		//Use the same cookies for all requests/responses
		private CookieContainer _cookies;

		private string _guidString;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public DatasourceExecutor(string GuidString)
		{
			_guidString = GuidString;
		}

		public void ExecutePendingJobs()
		{
			Webservices.TrayApplication ws;
			string requestKey;
			SerializableWebRequest currentRequest;
			HttpWebResponse httpResponse;
			SerializableWebResponse response;
			Random generator = new Random();

			ws = new Webservices.TrayApplication();
			ws.Url = "http://www.RankTrend.com/Webservices/TrayApplication.asmx";
			//ws.Url = "http://localhost:4933/Rt.Website/Webservices/TrayApplication.asmx";

			for (requestKey = ws.QueueNextDatasource(_guidString);
			     requestKey != null;
					 requestKey = ws.QueueNextDatasource(_guidString))
			{
				_log.DebugFormat("A request was queued as key '{0}'", requestKey);

				_cookies = new CookieContainer();

				for (currentRequest = ws.GetDatasourceRequest(requestKey);
				     currentRequest != null;
				     currentRequest = ws.GetDatasourceRequest(requestKey))
				{
					_log.DebugFormat("Executing a request for request key '{0}'", requestKey);

					httpResponse = ProcessRequest(currentRequest);
					response = WrapHttpResponse(httpResponse);

					_log.InfoFormat("Saving the response for request key '{0}'", requestKey);
					ws.SaveDatasourceResponse(response, requestKey);
				}

				// Random delay between 30 seconds and 3 minutes
				int delay = generator.Next(2 * 60 * 1000 + 30000) + 30000;		// Random number 0:00 - 2:30
				_log.DebugFormat("Waiting {0}ms before getting next pending datasource task.", delay);
				Thread.Sleep(delay);
			}
		}

		/// <param name="request">
		///		The information about the request that should be made.
		/// </param>
		/// <returns>
		///		The response from the server.
		/// </returns>
		public HttpWebResponse ProcessRequest(SerializableWebRequest request)
		{
			HttpWebRequest httpRequest;
			byte[] postBytes;
			Stream requestStream;

			httpRequest = (HttpWebRequest)HttpWebRequest.Create(request.Url);
			httpRequest.CookieContainer = _cookies;
			//Todo: randomize this
			httpRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.1) Gecko/20061204 Firefox/2.0.0.1";
			//We need this, particularly for Google login stuff
			httpRequest.AllowAutoRedirect = true;

			//Check if there is any data to post
			if (request.PostData != null)
			{
				//Set up the post data
				postBytes = Encoding.UTF8.GetBytes(request.PostData);

				httpRequest.Method = "POST";
				httpRequest.ContentType = "application/x-www-form-urlencoded";
				httpRequest.ContentLength = postBytes.Length;
				requestStream = httpRequest.GetRequestStream();
				requestStream.Write(postBytes, 0, postBytes.Length);
				requestStream.Close();
			}

			return (HttpWebResponse)httpRequest.GetResponse();
		}

		public SerializableWebResponse WrapHttpResponse(HttpWebResponse httpResponse)
		{
			StreamReader sr;
			SerializableWebResponse response;

			_log.Debug("Wrapping HttpWebResponse stream into its serializable form");

			using (sr = new StreamReader(httpResponse.GetResponseStream()))
			{
				response = new SerializableWebResponse();
				response.Content = sr.ReadToEnd();
			}

#if(DEBUG)
			_log.DebugFormat("Response contained {0}", response.Content);
#endif

			return response;
		}
	}
}