<%@ WebService Language="C#" Class="TrayApplication" %>

using System;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.Services;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services.DataEngine;
using Rt.Website;

[WebService(Namespace = "http://www.RankTrend.com/TrayAppplication")]
public class TrayApplication : WebService
{
	private const string KEY_PREFIX = "ds_ws_key_";

	/// <summary>
	///		Declare and create our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private readonly Database _db;
	private readonly DatasourceManager datasourceManager;

	public TrayApplication()
	{
		_db = Global.GetDbConnection();
		datasourceManager = new DatasourceManager();
	}

	/// <summary>
	///		Gets the minimum version number of the client that is required.
	/// </summary>
	/// <returns></returns>
	[WebMethod]
	public string GetMinimumClientVersion()
	{
		return "1.0.0.0";
	}

	[WebMethod]
	public string GetCurrentClientVersion()
	{
		//TODO: Read the MSI file and determine the current version?
		return "1.0.0.0";
	}

	[WebMethod]
	public string Authenticate(string userName, string password)
	{
		MembershipUser user;
		Guid userGuid;

		if (!Membership.ValidateUser(userName, password))
			return null;

		user = Membership.GetUser(userName);

		//Since we're storing the users in SQL, we can cast the user
		//ID to a Guid.
		userGuid = (Guid) user.ProviderUserKey;

		return userGuid.ToString();
	}

	[WebMethod]
	public void RecordError(string guid, string exceptionString)
	{
	}

	[WebMethod]
	public string QueueNextDatasource(string guidString)
	{
		int? configuredDatasourceId;
		ConfiguredDatasource cds;
		IDataSource currentDatasource = null;
		string newKey;
		Guid guid;
		DatasourceParameterType[] parameterTypes;

		try
		{
			_log.DebugFormat("Queueing next datasource for user {0}", guidString);

			guid = new Guid(guidString);

			//Get the next datasource for the user
			configuredDatasourceId = _db.GetNextConfiguredDatasourceId(guid);

			//Check if there are any queued datasources for the user
			if (configuredDatasourceId == null)
				return null;

			cds = _db.ORManager.Get<ConfiguredDatasource>(configuredDatasourceId);

			//Look up the datasource information
			currentDatasource = datasourceManager.GetDatasource(cds.DatasourceType.Id);
			if (currentDatasource == null)
			{
				_log.WarnFormat("Could not load an appropriate datasource for ID {0}", cds.DatasourceType.Id);
				return null;
			}

			//Load the parameter types
			parameterTypes = _db.GetParameterTypesForDatasource(cds.DatasourceType.Id);

			//Set up the datasource
			cds.InitializeDatasource(currentDatasource, parameterTypes);

			//Generate a unique key
			newKey = KEY_PREFIX + cds.Id;

			_log.DebugFormat("Generated a new key of '{0}' for the datasource request", newKey);

			//Queue the datasource so that we have it when we need to save the next requests and responses
			HttpRuntime.Cache.Add(newKey, currentDatasource, null, Cache.NoAbsoluteExpiration,
			                      TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);

			_log.Debug("The datasource has been queued");
		}
		catch (Exception ex)
		{
			_log.Error("There was a problem queuing a datasource for the client", ex);
			newKey = null;
		}

		return newKey;
	}

	[WebMethod]
	public SerializableWebRequest GetDatasourceRequest(string requestKey)
	{
		IDataSource currentDatasource;
		SerializableWebRequest nextRequest;

		currentDatasource = HttpRuntime.Cache.Get(requestKey) as IDataSource;

		if (currentDatasource == null)
		{
			//If the datasource isn't in the cache, it must be done
			_log.DebugFormat("Datasource with the key {0} must be complete, since it is no longer cached", requestKey);
			return null;
		}

		try
		{
			nextRequest = currentDatasource.GetNextRequest();
		}
		catch (Exception ex)
		{
			_log.Error(string.Format("There was a problem getting the next datasource request for key '{0}'", requestKey), ex);
			nextRequest = null;
		}

		return nextRequest;
	}

	[WebMethod]
	public void SaveDatasourceResponse(SerializableWebResponse response, string requestKey)
	{
		IDataSource currentDatasource;
		int configuredDatasourceId;
		RawDataValue[] values;

		configuredDatasourceId = int.Parse(requestKey.Replace(KEY_PREFIX, ""));
		currentDatasource = HttpRuntime.Cache.Get(requestKey) as IDataSource;

		if (currentDatasource == null)
		{
			_log.WarnFormat(
				"A client is trying to save a response for '{0}', but the server doesn't have it in the request list", requestKey);
			return;
		}

		try
		{
			currentDatasource.SetResponse(response);

			//Query the value to see if one exists
			values = currentDatasource.Values;
			if (values != null && values.Length > 0)
			{
				foreach (RawDataValue currValue in values)
				{
					string context = null;

					currValue.ConfiguredDatasourceId = configuredDatasourceId;

					//If the read failed or was fuzzy, save the context to the queue
					//so that we can process it later.
					if (!currValue.Success || currValue.Fuzzy)
						context = response.Content;

					_db.InsertRawData(currValue, context);
				}

				//Now that the datasource is complete, we can remove it
				HttpRuntime.Cache.Remove(requestKey);
				_log.DebugFormat("Removed data source key {0}, because it's done executing", requestKey);
			}
		}
		catch (Exception ex)
		{
			HttpRuntime.Cache.Remove(requestKey);
			_log.DebugFormat("Removed data source key {0}", requestKey);
			_log.Error(string.Format("There was a problem saving the results for request key '{0}'", requestKey), ex);
		}
	}
}