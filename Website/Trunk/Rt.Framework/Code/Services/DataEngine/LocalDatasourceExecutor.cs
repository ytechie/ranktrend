using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Rt.Framework.Db;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;
using Rt.Framework.Services.DataEngine;
using Rt.Framework.Services.DataEngine.DataSources;
using YTech.Db;
using log4net;
using System.Reflection;

namespace Rt.Framework.Services.DataEngine
{
	/// <summary>
	///		This class is designed to directly use an
	///		<see cref="IDataSource" />, and retreive the
	///		data it was designed to get.
	/// </summary>
	public class LocalDatasourceExecutor
	{
		IDataSource _datasource;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		CookieContainer cookies;

		public LocalDatasourceExecutor(IDataSource datasource)
		{
			_datasource = datasource;
		}

		public LocalDatasourceExecutor()
		{
		}

		/// <summary>
		///		Retreives the data from the datasource.
		/// </summary>
		public void Execute()
		{
			SerializableWebRequest request;
			SerializableWebResponse response;
			WebResponse httpResponse;
			StreamReader sr;

			//Use a new cookie container for each datasource
			cookies = new CookieContainer();

			for (request = _datasource.GetNextRequest(); request != null; request = _datasource.GetNextRequest())
			{
				httpResponse = ProcessRequest(request);
				
				using (sr = new StreamReader(httpResponse.GetResponseStream()))
				{
					response = new SerializableWebResponse();
					response.Content = sr.ReadToEnd();
				}

				_datasource.SetResponse(response);
			}
		}

		/// <summary>
		///		This processes the request LOCALLY, which is usually
		///		not where the processing should be done.  This would
		///		normally be done on the client.
		/// </summary>
		/// <remarks>
		///		The primary purpose of this method is to test datasources
		///		locally before having clients complicate the process.
		/// </remarks>
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
			httpRequest.CookieContainer = cookies;
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

		public static void RunPendingJobs(Database db)
		{
			LocalDatasourceExecutor jobExecutor;
			IDataSource ds;
			int? nextDatasourceId;
			DatasourceManager datasourceManager;
			ConfiguredDatasource currentDatasource;
			RawDataValue[] values;
			DatasourceParameterType[] parameterTypes;

			_log.Debug("Creating the datasource manager to load the datasources using reflection");
			datasourceManager = new DatasourceManager();

			_log.Debug("Checking for available jobs to run");
			while ((nextDatasourceId = db.GetNextConfiguredDatasourceId()) != null)
			{
				_log.DebugFormat("Found configured datasource ID #{0} to run", nextDatasourceId);
				currentDatasource = db.ORManager.Get<ConfiguredDatasource>(nextDatasourceId);
				_log.DebugFormat("Loaded datasource; type={1}", currentDatasource.DatasourceType.Id);

				try
				{
					//Load the datasource class to process the configured datasource
					ds = datasourceManager.GetDatasource(currentDatasource.DatasourceType.Id);
					if (ds == null)
					{
						_log.WarnFormat("Could not load an appropriate datasource reader for ID {0}", currentDatasource.DatasourceType.Id);
						return;
					}

					//Load the parameter types
					parameterTypes = db.GetParameterTypesForDatasource(currentDatasource.DatasourceType.Id);

					//Set up the datasource
					currentDatasource.InitializeDatasource(ds, parameterTypes);

					jobExecutor = new LocalDatasourceExecutor(ds);
					jobExecutor.Execute();

					//Set the configured datasource ID on all the values
					values = ds.Values;
					if (values != null && values.Length > 0)
					{
						foreach (RawDataValue currValue in values)
							currValue.ConfiguredDatasourceId = nextDatasourceId.Value;

						//Save the results
						db.BulkInsertRawData(values);
					}
				}
				catch(Exception ex)
				{
					_log.Error(string.Format("Error executing configured datasource id #{0}", nextDatasourceId), ex);
				}
			}
		}
	}
}
