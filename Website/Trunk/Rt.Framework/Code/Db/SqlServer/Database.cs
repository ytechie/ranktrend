using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using YTech.Db.SqlServer;
using log4net;
using System.Reflection;
using Rt.Framework.Components;
using YTech.General.DataMapping;
using Rt.Framework.Db.Exceptions;
using NHibernate;
using NHibernate.Expression;
using YTech.Db;

namespace Rt.Framework.Db.SqlServer
{
	/// <summary>
	///		Provides all of the functionality to use SQL Server
	///		as the data store for the application.
	/// </summary>
	public class Database
	{
		#region Private Properties

		private YTech.Db.SqlServer.DbConnection _dbConn;
		private ORManager _orDBConn;

		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#endregion

		#region Constructors

		/// <summary>
		///		Creates a new instance of the <see cref="Database"/> class.
		/// </summary>
		/// <param name="dbConn"></param>
		public Database(YTech.Db.SqlServer.DbConnection dbConn, ORManager orDBConn)
		{
			_dbConn = dbConn;
			_orDBConn = orDBConn;
		}
		
		#endregion

		public ORManager ORManager
		{
			get { return _orDBConn; }
		}

		public YTech.Db.SqlServer.DbConnection DbConn
		{
			get { return _dbConn; }
		}

		#region Global Settings

		/// <summary>
		///		Populates the specified <see cref="GlobalSetting"/>.
		/// </summary>
		/// <param name="globalSetting">The <see cref="GlobalSetting"/> that should be populated.</param>
		/// <exception cref="GlobalSettingNotFound">
		///		Thrown when the identifier of the global setting could not be found
		///		in the database.
		/// </exception>
		public void PopulateGlobalSetting(GlobalSetting globalSetting)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("GetGlobalSetting");
			cmd.Parameters.AddWithValue("@GlobalSettingId", globalSetting.Id);

			ds = _dbConn.GetDataSet(cmd);

			GlobalSettingNotFound noDataEx = new GlobalSettingNotFound(globalSetting.Id);

			DataMapper.PopulateObject(globalSetting, ds, noDataEx, null);
		}

		/// <summary>
		///		Loads the setting from the database with the specified ID
		/// </summary>
		///	<param name="id">
		///		The unique identifier of the setting in the database.
		///	</param>
		/// <exception cref="GlobalSettingNotFound">
		///		Thrown when the identifier of the global setting could not be found
		///		in the database.
		/// </exception>
		public GlobalSetting GetGlobalSetting(int id)
		{
			GlobalSetting gs;

			gs = new GlobalSetting(id);
			PopulateGlobalSetting(gs);

			return gs;
		}

		#endregion

		/// <summary>
		///		Gets the UTC <see cref="DateTime"/> of the database server.
		/// </summary>
		/// <remarks>
		///		It's important to have a single source for the current time. You
		///		should always use this to stay in sync.
		/// </remarks>
		/// <returns></returns>
		public DateTime GetServerTime()
		{
			SqlCommand cmd;
			DataSet ds;
			object cellValue;

			cmd = _dbConn.GetSqlCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "Select GetUtcDate()";

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				throw new Exception("No data returned when calling the server function to retrieve the date");

			cellValue = ds.Tables[0].Rows[0][0];

			if (cellValue == null || cellValue == DBNull.Value)
				throw new Exception("NULL value returned when calling the server function to retrieve the date");

			return (DateTime)cellValue;
		}

		#region Urls

		public UrlClass GetUrl(int urlId)
		{
			SqlCommand cmd;
			DataSet ds;
			UrlClass u;

			cmd = _dbConn.GetStoredProcedureCommand("Url_GetUrl");
			cmd.Parameters.AddWithValue("@UrlId", urlId);
			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables[0].Rows.Count == 0)
				return null;

			u = new UrlClass();

			DataMapper.PopulateObject(u, ds, null, null);

			return u;
		}

		public UrlClass[] GetUserUrls(Guid userId)
		{
			SqlCommand cmd;
			DataSet ds;
			object[] objects;
			UrlClass[] urls;

			cmd = _dbConn.GetStoredProcedureCommand("Urls_GetUserUrls");
			cmd.Parameters.AddWithValue("@UserId", userId);
			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables[0].Rows.Count == 0)
				return new UrlClass[0];

			objects = DataMapper.CreateObjects(ds, typeof(UrlClass));
			urls = new UrlClass[objects.Length];
			objects.CopyTo(urls, 0);

			return urls;
		}

		#endregion

		#region Datasources

		public void DeleteConfiguredDatasource(int configuredDatasourceId)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Ds_DeleteDatasource");
			cmd.Parameters.AddWithValue("@ConfiguredDatasourceId", configuredDatasourceId);

			_dbConn.ExecuteNonQuery(cmd);
		}

		public DataTable QueryRawData(DateTime start, DateTime end, int configuredDatasourceId, int? dataSubTypeId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Ds_QueryRawData");
			cmd.Parameters.AddWithValue("@Start", start).DbType = DbType.DateTime;
			cmd.Parameters.AddWithValue("@End", end).DbType = DbType.DateTime;
			cmd.Parameters.AddWithValue("@ConfiguredDatasourceId", configuredDatasourceId);
			cmd.Parameters.AddWithValue("@DataSubTypeId", dataSubTypeId);

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		/// <summary>
		///		Gets the Id of the next configured datasource id that needs to
		///		have its data updated.
		/// </summary>
		public int? GetNextConfiguredDatasourceId()
		{
			return GetNextConfiguredDatasourceId(null);
		}

		/// <summary>
		///		Gets the Id of the next configured datasource id that needs to
		///		have its data updated for the specified user.
		/// </summary>
		public int? GetNextConfiguredDatasourceId(Guid? userId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Ds_GetNextConfiguredDatasourceId");
			cmd.Parameters.AddWithValue("@UserId", userId);
			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0] == DBNull.Value)
				return null;
			
			return (int?)ds.Tables[0].Rows[0][0];
		}

		/// <summary>
		///		Gets a list of datasources that gets displayed in the
		///		interactive report list.  It does the work of expanding the
		///		datasource sub types into a simple datasource list.
		/// </summary>
		/// <param name="userId">
		///		The user Id of the user that requires the list to be displayed.
		/// </param>
		/// <returns>
		///		A table that is meant do be bound in a data grid to list the datasources.
		/// </returns>
		public DataTable GetTrendDatasoureList(Guid userId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Ds_GetTrendDatasourceList");
			cmd.Parameters.AddWithValue("@UserId", userId);
			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		/// <summary>
		///		Gets a datatable for the EventTable control for the
		///		Interactive report.
		/// </summary>
		/// <param name="userId">
		///		The ID of the user that the event category list is being retrieved for.
		/// </param>
		/// <returns></returns>
		public DataTable IR_GetEventCategoryList(Guid userId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("IR_GetEventCategoryList");
			cmd.Parameters.AddWithValue("@UserId", userId);
			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		///// <summary>
		/////   Retrieves an object of type <see cref="DatasourceParameter" /> from the database.
		///// </summary>
		/////	<returns>
		/////		An array of DatasourceParameter objects created from the database
		/////		call.  If no data is returned from the database, the array will
		/////		be an array of size 0.
		/////	</returns>
		//public DatasourceParameterType[] GetDatasourceParameterList(int datasourceTypeId)
		//{
		//  SqlCommand cmd;
		//  DataSet ds;
		//  object[] objects; //Temporary stores the created objects
		//  DatasourceParameterType[] stronglyTypedArray;

		//  cmd = _dbConn.GetStoredProcedureCommand("Ds_GetParameterList");
		//  cmd.Parameters.AddWithValue("@DatasourceTypeId", datasourceTypeId);

		//  ds = _dbConn.GetDataSet(cmd);

		//  if (ds.Tables.Count == 0)
		//    return new DatasourceParameterType[0];
		//  if (ds.Tables[0].Rows.Count == 0)
		//    return new DatasourceParameterType[0];

		//  //Use the DataMapper to create the objects into a temporary array
		//  objects = DataMapper.CreateObjects(ds, typeof(DatasourceParameterType));

		//  //Create the actual array of the correct type, and fill it with the objects
		//  stronglyTypedArray = new DatasourceParameterType[objects.Length];
		//  objects.CopyTo(stronglyTypedArray, 0);

		//  return stronglyTypedArray;
		//}

		/// <summary>
		///		Gets a list of datasources by the specified page Id.  This is
		///		used to populate the configured datasource lists.
		/// </summary>
		/// <param name="urlId"></param>
		/// <returns></returns>
		public DataTable Ds_GetPageDatasourceList(int urlId)
		{
			return Ds_GetPageDatasourceList(urlId, false);
		}

		/// <summary>
		///		Gets a list of datasources by the specified page Id.  This is
		///		used to populate the configured datasource lists.
		/// </summary>
		/// <param name="urlId"></param>
		/// <returns></returns>
		public DataTable Ds_GetPageDatasourceList(int urlId, bool includeSubtypes)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Ds_GetPageDatasourceList");
			cmd.Parameters.AddWithValue("@UrlId", urlId);
			cmd.Parameters.AddWithValue("@IncludeSubtypes", includeSubtypes);

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		public DatasourceParameterType[] GetParameterTypesForDatasource(int datasourceTypeId)
		{
			ICriteria criteria;
			IList<DatasourceParameterType> parameterTypes;
			DatasourceParameterType[] parameterArr;

			criteria = _orDBConn.Session.CreateCriteria(typeof(DatasourceParameterType)).Add(Expression.Eq("DatasourceType.Id", datasourceTypeId));
			parameterTypes = criteria.List<DatasourceParameterType>();

			parameterArr = new DatasourceParameterType[parameterTypes.Count];
			parameterTypes.CopyTo(parameterArr, 0);

			return parameterArr;
		}

		public DataTable Report_DatasourceCorrelations(int urlId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Report_DatasourceCorrelations");
			cmd.Parameters.AddWithValue("@UrlId", urlId);

			ds = _dbConn.GetDataSet(cmd);
			return ds.Tables[0];
		}

		public DataTable GetQueuedDatasourceData()
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetSqlCommand();
			cmd.CommandText = "Select Top 50 Id From rt_DatasourceDataQueue dq Order By LastAttempt Desc";
			cmd.CommandType = CommandType.Text;

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		#endregion

		#region Services

		///// <summary>
		/////     Gets the information about a particular Windows service.
		///// </summary>
		///// <param name="serviceId">
		/////     The unique ID of the service whose information you would like to load.
		///// </param>
		///// <returns>
		/////     A <see cref="Service"/> object with the ID of the service you are
		/////     requesting.
		///// </returns>
		//public Service GetService(int serviceId)
		//{
		//  SqlCommand cmd;
		//  DataSet ds;
		//  Service svc;

		//  cmd = _dbConn.GetStoredProcedureCommand("Services_GetService");
		//  cmd.Parameters.AddWithValue("@ServiceId", serviceId);

		//  ds = _dbConn.GetDataSet(cmd);

		//  svc = new Service();
		//  DataMapper.PopulateObject(svc, ds, null, null);

		//  return svc;
		//}

		///// <summary>
		/////     Adds or updates the <see cref="Service"/> in the database.
		///// </summary>
		///// <param name="svc"></param>
		//public void SaveService(Service svc)
		//{
		//  SqlCommand cmd;
		//  SqlParameter returnParam;

		//  cmd = _dbConn.GetStoredProcedureCommand("Services_SaveService");

		//  if (svc.HasId)
		//    cmd.Parameters.AddWithValue("@ServiceId", svc.Id);

		//  cmd.Parameters.AddWithValue("@Description", svc.Description);
		//  cmd.Parameters.AddWithValue("@LastHeartbeat", svc.LastHeartbeat);
		//  cmd.Parameters.AddWithValue("@RunIntervalMinutes", svc.RunIntervalMinutes);
		//  cmd.Parameters.AddWithValue("@Enabled", svc.Enabled);
		//  cmd.Parameters.AddWithValue("@LastRunTime", svc.LastRunTime);
		//  cmd.Parameters.AddWithValue("@ReloadConfiguration", svc.ReloadConfiguration);
		//  cmd.Parameters.AddWithValue("@ForceRun", svc.ForceRun);

		//  //Return value
		//  returnParam = cmd.Parameters.AddWithValue("@Return_Value", SqlDbType.Int);
		//  returnParam.Direction = ParameterDirection.ReturnValue;

		//  _dbConn.ExecuteNonQuery(cmd);

		//  //If we did an insert, grab the new id
		//  if (!svc.HasId)
		//    svc.Id = (int)returnParam.Value;
		//}

		///// <summary>
		/////     Updates the last heartbeat time for a service.
		///// </summary>
		///// <param name="serviceId">
		/////     The ID of the service to update.
		///// </param>
		///// <param name="ReloadConfiguration">
		/////     A boolean indicating wheter or not the service information
		/////     needs to be reloaded from the database.  Once this value is read,
		/////     it gets reset to 0, so you better handle it.
		///// </param>
		//public void SaveServiceHeartbeat(int serviceId, out bool ReloadConfiguration)
		//{
		//  SqlCommand cmd;
		//  SqlParameter reloadParam;

		//  cmd = _dbConn.GetStoredProcedureCommand("Services_SaveHeartbeat");
		//  cmd.Parameters.AddWithValue("@ServiceId", serviceId);

		//  reloadParam = new SqlParameter("@ReloadConfiguration", SqlDbType.Bit);
		//  reloadParam.Direction = ParameterDirection.Output;
		//  cmd.Parameters.Add(reloadParam);

		//  _dbConn.ExecuteNonQuery(cmd);

		//  ReloadConfiguration = (bool)reloadParam.Value;
		//}

		#endregion

		#region Email

		/// <summary>
		///		Gets the next email from the queue that should be sent.
		/// </summary>
		/// <returns></returns>
		public EmailMessage GetNextQueuedEmail()
		{
			SqlCommand cmd;
			DataSet ds;
			EmailMessage msg;

			cmd = _dbConn.GetStoredProcedureCommand("Email_GetNextMessageFromQueue");
			ds = _dbConn.GetDataSet(cmd);

			//If we can't find an email, just return null
			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				return null;

			msg = new EmailMessage();
			DataMapper.PopulateObject(msg, ds, null, null);

			return msg;
		}

		/// <summary>
		///		Saves the specified message to the database.
		/// </summary>
		/// <param name="msg">
		///		The message to save to the database.
		/// </param>
		public void SaveEmail(EmailMessage msg)
		{
			SqlCommand cmd;
			SqlParameter returnParam;
			DateTime nullDate;

			cmd = _dbConn.GetStoredProcedureCommand("Email_SaveEmailMessage");

			if (msg.Id != null)
				cmd.Parameters.AddWithValue("@EmailId", msg.Id);

			nullDate = new DateTime();

			cmd.Parameters.AddWithValue("@From", msg.From);
			cmd.Parameters.AddWithValue("@ToName", msg.ToName);
			cmd.Parameters.AddWithValue("@ToAddress", msg.ToAddress);
			cmd.Parameters.AddWithValue("@Subject", msg.Subject);
			cmd.Parameters.AddWithValue("@Message", msg.Message);
			cmd.Parameters.AddWithValue("@Html", msg.Html);
			if (msg.SentOn != nullDate) cmd.Parameters.AddWithValue("@SentOn", msg.SentOn);
			if (msg.QueuedOn != nullDate) cmd.Parameters.AddWithValue("@QueuedOn", msg.QueuedOn);
			if (msg.LastTry != nullDate) cmd.Parameters.AddWithValue("@LastTry", msg.LastTry);
			cmd.Parameters.AddWithValue("@NumberOfTries", msg.NumberOfTries);
			cmd.Parameters.AddWithValue("@UserId", msg.UserId);

			//Return value
			returnParam = cmd.Parameters.AddWithValue("@Return_Value", SqlDbType.Int);
			returnParam.Direction = ParameterDirection.ReturnValue;

			_dbConn.ExecuteNonQuery(cmd);

			//If we did an insert, grab the new id
			if (msg.Id == null)
				msg.Id = (int)returnParam.Value;
		}

		public EmailTemplate GetEmailTemplate(int id)
		{
			SqlCommand cmd;
			DataSet data;
			EmailTemplate emailTemplate = null;

			cmd = _dbConn.GetStoredProcedureCommand("Email_GetEmailTemplate");
			cmd.Parameters.AddWithValue("@Id", id);

			data = _dbConn.GetDataSet(cmd);
			if (data.Tables[0].Rows.Count == 1)
			{
				emailTemplate = new EmailTemplate();
				DataMapper.PopulateObject(emailTemplate, data.Tables[0].Rows[0], null);
			}

			return emailTemplate;
		}

		public Dictionary<int, EmailTemplate> GetEmailTemplates()
		{
			DataTable data;
			Dictionary<int, EmailTemplate> emailTemplates = new Dictionary<int, EmailTemplate>();
			EmailTemplate emailTemplate;

			data = GetEmailTemplatesTable();
			foreach (DataRow dr in data.Rows)
			{
				emailTemplate = new EmailTemplate();
				DataMapper.PopulateObject(emailTemplate, dr, null);
				emailTemplates.Add((int)emailTemplate.Id, emailTemplate);
			}

			return emailTemplates;
		}

		public DataTable GetEmailTemplatesTable()
		{
			SqlCommand cmd;
			DataSet data;

			cmd = _dbConn.GetStoredProcedureCommand("Email_GetEmailTemplates");

			data = _dbConn.GetDataSet(cmd);

			return data.Tables[0];
		}

		public void SaveEmailTemplate(EmailTemplate emailTemplate)
		{
			SqlCommand cmd;
			SqlParameter returnParam;

			cmd = _dbConn.GetStoredProcedureCommand("Email_SaveEmailTemplate");

			if (emailTemplate.Id != null)
				cmd.Parameters.AddWithValue("@EmailTemplateId", emailTemplate.Id);

			cmd.Parameters.AddWithValue("@Subject", emailTemplate.Subject);
			cmd.Parameters.AddWithValue("@Message", emailTemplate.Message);
			cmd.Parameters.AddWithValue("@Html", emailTemplate.Html);
			cmd.Parameters.AddWithValue("@Locked", emailTemplate.Locked);

			//Return value
			returnParam = cmd.Parameters.AddWithValue("@Return_Value", SqlDbType.Int);
			returnParam.Direction = ParameterDirection.ReturnValue;

			_dbConn.ExecuteNonQuery(cmd);

			//If we did an insert, grab the new id
			if (emailTemplate.Id == null)
				emailTemplate.Id = (int)returnParam.Value;
		}

		public void DeleteEmailTemplate(int emailTemplateId)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Email_DeleteEmailTemplate");

			cmd.Parameters.AddWithValue("@EmailTemplateId", emailTemplateId);

			_dbConn.ExecuteNonQuery(cmd);
		}

		public void SendMassEmail(int emailTemplateId, string fromAddress, string to, string applicationName)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Email_SendMassEmail");

			cmd.Parameters.AddWithValue("@EmailTemplateId", emailTemplateId);
			cmd.Parameters.AddWithValue("@FromAddress", fromAddress);
			cmd.Parameters.AddWithValue("@RoleName", to);
			cmd.Parameters.AddWithValue("@ApplicationName", applicationName);

			_dbConn.ExecuteNonQuery(cmd);
		}

		#endregion

		#region Legal Notices
		public LegalNotice GetLegalNotice(int noticeId)
		{
			SqlCommand cmd;
			DataSet ds;
			LegalNotice notice;

			cmd = _dbConn.GetStoredProcedureCommand("Legal_GetNotice");
			cmd.Parameters.AddWithValue("@Id", noticeId);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				throw new ApplicationException("There are no versions for the specified legal notice.");

			notice = new LegalNotice();
			DataMapper.PopulateObject(notice, ds, null, null);

			return notice;
		}

		public LegalNoticeVersion GetLatestLegalNoticeVersion(int legalNoticeId)
		{
			SqlCommand cmd;
			DataSet ds;
			LegalNoticeVersion version;

			cmd = this._dbConn.GetStoredProcedureCommand("Legal_GetNoticeVersion");
			cmd.Parameters.AddWithValue("@NoticeId", legalNoticeId);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				return null;

			version = new LegalNoticeVersion();
			DataMapper.PopulateObject(version, ds, null, null);

			return version;
		}

		public bool HasAgreedToNotice(int legalNoticeVersionId, object userId)
		{
			SqlCommand cmd;
			DataSet ds;
			LegalNoticeAgreement agreement;

			cmd = _dbConn.GetStoredProcedureCommand("Legal_GetNoticeAgreement");
			cmd.Parameters.AddWithValue("@NoticeVersionId", legalNoticeVersionId);
			cmd.Parameters.AddWithValue("@UserId", userId);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				return false;
			else
			{
				agreement = new LegalNoticeAgreement();
				DataMapper.PopulateObject(agreement, ds, null, null);
				return agreement.Agree;
			}
		}

		public void SaveLegalNoticeAgreement(LegalNoticeAgreement agreement)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Legal_SaveNoticeAgreement");
			cmd.Parameters.AddWithValue("@NoticeVersionId", agreement.LegalNoticeVersionId);
			cmd.Parameters.AddWithValue("@UserId", agreement.UserId);
			cmd.Parameters.AddWithValue("@Agree", agreement.Agree);

			_dbConn.ExecuteNonQuery(cmd);
		}

		public void SaveLegalNoticeVersion(LegalNoticeVersion legalNoticeVersion)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Legal_SaveNoticeVersion");
			cmd.Parameters.AddWithValue("@NoticeId", legalNoticeVersion.LegalNoticeId);
			cmd.Parameters.AddWithValue("@Notice", legalNoticeVersion.Notice);

			_dbConn.ExecuteNonQuery(cmd);
		}
		#endregion

		#region Services

		/// <summary>
		///     Gets the information about a particular Windows service.
		/// </summary>
		/// <param name="serviceId">
		///     The unique ID of the service whose information you would like to load.
		/// </param>
		/// <returns>
		///     A <see cref="Service"/> object with the ID of the service you are
		///     requesting.
		/// </returns>
		public Service GetService(int serviceId)
		{
			SqlCommand cmd;
			DataSet ds;
			Service svc;

			cmd = _dbConn.GetStoredProcedureCommand("Services_GetService");
			cmd.Parameters.AddWithValue("@ServiceId", serviceId);

			ds = _dbConn.GetDataSet(cmd);

			svc = new Service();
			DataMapper.PopulateObject(svc, ds, null, null);

			return svc;
		}

		/// <summary>
		///     Adds or updates the <see cref="Service"/> in the database.
		/// </summary>
		/// <param name="svc"></param>
		public void SaveService(Service svc)
		{
			SqlCommand cmd;
			SqlParameter returnParam;

			cmd = _dbConn.GetStoredProcedureCommand("Services_SaveService");

			if (svc.Id != null)
				cmd.Parameters.AddWithValue("@ServiceId", svc.Id);

			cmd.Parameters.AddWithValue("@Description", svc.Description);
			cmd.Parameters.AddWithValue("@RunIntervalMinutes", svc.RunIntervalMinutes);
			cmd.Parameters.AddWithValue("@Enabled", svc.Enabled);
			cmd.Parameters.AddWithValue("@ReloadConfiguration", svc.ReloadConfiguration);
			cmd.Parameters.AddWithValue("@ForceRun", svc.ForceRun);
			cmd.Parameters.AddWithValue("@Owner", svc.Owner);

			//Return value
			returnParam = cmd.Parameters.AddWithValue("@Return_Value", SqlDbType.Int);
			returnParam.Direction = ParameterDirection.ReturnValue;

			_dbConn.ExecuteNonQuery(cmd);

			//If we did an insert, grab the new id
			if (svc.Id == null)
				svc.Id = (int)returnParam.Value;
		}

		/// <summary>
		///     Updates the last heartbeat time for a service.
		/// </summary>
		/// <param name="serviceId">
		///     The ID of the service to update.
		/// </param>
		/// <param name="ReloadConfiguration">
		///     A boolean indicating wheter or not the service information
		///     needs to be reloaded from the database.  Once this value is read,
		///     it gets reset to 0, so you better handle it.
		/// </param>
		/// <param name="forceRun">
		///     A boolean indicating wheter or not the service should
		///     force a run now.  Once this value is read,
		///     it gets reset to 0, so you better handle it.
		/// </param>
		public void SaveServiceHeartbeat(int serviceId, out bool reloadConfiguration, out bool forceRun)
		{
			SqlCommand cmd;
			SqlParameter reloadParam, runParam;

			cmd = _dbConn.GetStoredProcedureCommand("Services_SaveHeartbeat");
			cmd.Parameters.AddWithValue("@ServiceId", serviceId);

			reloadParam = new SqlParameter("@ReloadConfiguration", SqlDbType.Bit);
			reloadParam.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(reloadParam);

			runParam = new SqlParameter("@ForceRun", SqlDbType.Bit);
			runParam.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(runParam);

			_dbConn.ExecuteNonQuery(cmd);

			reloadConfiguration = (bool)reloadParam.Value;
			forceRun = (bool)runParam.Value;
		}

		/// <summary>
		///     Updates the last run time for a service.
		/// </summary>
		/// <param name="serviceId">
		///     The ID of the service to update.
		/// </param>
		/// <param name="ReloadConfiguration">
		///     A boolean indicating wheter or not the service information
		///     needs to be reloaded from the database.  Once this value is read,
		///     it gets reset to 0, so you better handle it.
		/// </param>
		/// <param name="forceRun">
		///     A boolean indicating wheter or not the service should
		///     force a run now.  Once this value is read,
		///     it gets reset to 0, so you better handle it.
		/// </param>
		public void SaveServiceRunTime(int serviceId, out bool reloadConfiguration, out bool forceRun)
		{
			SqlCommand cmd;
			SqlParameter reloadParam, runParam;

			cmd = _dbConn.GetStoredProcedureCommand("Services_SetLastRunTime");
			cmd.Parameters.AddWithValue("@ServiceId", serviceId);

			reloadParam = new SqlParameter("@ReloadConfiguration", SqlDbType.Bit);
			reloadParam.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(reloadParam);

			runParam = new SqlParameter("@ForceRun", SqlDbType.Bit);
			runParam.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(runParam);

			_dbConn.ExecuteNonQuery(cmd);

			reloadConfiguration = (bool)reloadParam.Value;
			forceRun = (bool)runParam.Value;
		}

		#endregion
		
		#region Plans

		public DataSet GetPlans()
		{
			SqlCommand cmd = _dbConn.GetStoredProcedureCommand("Plans_GetPlans");

			return _dbConn.GetDataSet(cmd);
		}

		public int Plans_GetUsersPlanId(Guid userId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Plans_GetUsersPlanId");
			cmd.Parameters.AddWithValue("@UserId", userId);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables[0].Rows.Count == 0)
				return 1;
			else
				return (int)ds.Tables[0].Rows[0][0];
		}

		#endregion

		#region Interactive Report

		/// <summary>
		///		Gets a list of search engines that have datasources configured
		///		for a specified page, for a specific user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="pageId"></param>
		/// <returns></returns>
		public SearchEngine[] IRGetSearchEngineList(Guid userId, int? pageId)
		{
			SqlCommand cmd;
			DataSet ds;
			object[] objects;
			SearchEngine[] searchEngines;

			cmd = _dbConn.GetStoredProcedureCommand("IR_GetSearchEngineList");
			cmd.Parameters.AddWithValue("@UserId", userId);
			cmd.Parameters.AddWithValue("@UrlId", pageId);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0)
				return new SearchEngine[0];
			if (ds.Tables[0].Rows.Count == 0)
				return new SearchEngine[0];

			objects = DataMapper.CreateObjects(ds, typeof(SearchEngine));
			searchEngines = new SearchEngine[objects.Length];
			objects.CopyTo(searchEngines, 0);

			return searchEngines;
		}

		/// <summary>
		///		Gets the Id of the next custom report id that needs to sent.
		/// </summary>
		public int? GetNextCustomReportId()
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("CustomReports_GetNextCustomReportId");
			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0] == DBNull.Value)
				return null;

			return (int?)ds.Tables[0].Rows[0][0];
		}

		#endregion

		#region Events
		
		/// <summary>
		///		Gets an unordered list of events for a time range and event category.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="eventCategoryId"></param>
		/// <returns></returns>
		public DataTable GetEventsByCategory(DateTime start, DateTime end, int eventCategoryId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Events_GetEventsByCategory");
			cmd.Parameters.AddWithValue("@Start", start).DbType = DbType.DateTime;
			cmd.Parameters.AddWithValue("@End", end).DbType = DbType.DateTime;
			cmd.Parameters.AddWithValue("@EventCategoryId", eventCategoryId);

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		/// <summary>
		///		Deletes an event category and all associated events.
		/// </summary>
		/// <param name="eventCategoryId"></param>
		public void DeleteEventCategory(int eventCategoryId)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Events_DeleteCategory");
			cmd.Parameters.AddWithValue("@EventCategoryId", eventCategoryId);

			_dbConn.ExecuteNonQuery(cmd);
		}

		#endregion

		public void InsertRawData(RawDataValue dataValue, string context)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("RawData_InsertRawDataValue");
			cmd.Parameters.AddWithValue("@ConfiguredDatasourceId", dataValue.ConfiguredDatasourceId);
			cmd.Parameters.AddWithValue("@DatasourceSubTypeId", dataValue.DatasourceSubTypeId);
			cmd.Parameters.AddWithValue("@Timestamp", dataValue.Timestamp);
			cmd.Parameters.AddWithValue("@FloatValue", dataValue.FloatValue);
			cmd.Parameters.AddWithValue("@Success", dataValue.Success);
			cmd.Parameters.AddWithValue("@Fuzzy", dataValue.Fuzzy);
			cmd.Parameters.AddWithValue("@SourceData", context);

			_dbConn.ExecuteNonQuery(cmd);
		}

		/// <summary>
		///		Performs a high performance insert of data values.
		/// </summary>
		/// <param name="dataValues"></param>
		public void BulkInsertRawData(RawDataValue[] dataValues)
		{
			SqlBulkCopy sbc;
			DataTable insertTable;

			insertTable = new DataTable();
			insertTable.Columns.Add("Id", typeof(int));
			insertTable.Columns.Add("ConfiguredDatasourceId", typeof(int));
			insertTable.Columns.Add("DatasourceSubTypeId", typeof(int));
			insertTable.Columns.Add("Timestamp", typeof(DateTime));
			insertTable.Columns.Add("FloatValue", typeof(double));
			insertTable.Columns.Add("Success", typeof(bool));
			insertTable.Columns.Add("Fuzzy", typeof(bool));

			foreach (RawDataValue currRawDataValue in dataValues)
			{
				object[] rowValues;

				rowValues = new object[insertTable.Columns.Count];
				rowValues[0] = currRawDataValue.Id;
				rowValues[1] = currRawDataValue.ConfiguredDatasourceId;
				if (currRawDataValue.DatasourceSubTypeId == null)
					rowValues[2] = DBNull.Value;
				else
					rowValues[2] = currRawDataValue.DatasourceSubTypeId;
				rowValues[3] = currRawDataValue.Timestamp;
				if (currRawDataValue.FloatValue == null)
					rowValues[4] = DBNull.Value;
				else
					rowValues[4] = currRawDataValue.FloatValue;
				rowValues[5] = currRawDataValue.Success;
				rowValues[6] = currRawDataValue.Fuzzy;

				insertTable.Rows.Add(rowValues);
			}

			using (sbc = _dbConn.GetBulkCopyObject(SqlBulkCopyOptions.FireTriggers))
			{
				sbc.DestinationTableName = "RawData";
				sbc.WriteToServer(insertTable);
			}
		}

		public void InsertRssEvent(int eventCategoryId, string name, string description, DateTime startTime, int urlId, string eventLink, string hash)
		{
			SqlCommand cmd = _dbConn.GetStoredProcedureCommand("Events_SetRssEvent");

			cmd.Parameters.AddWithValue("@EventCategoryId", eventCategoryId);
			cmd.Parameters.AddWithValue("@Name", name);
			cmd.Parameters.AddWithValue("@Description", description);
			cmd.Parameters.AddWithValue("@StartTime", startTime);
			cmd.Parameters.AddWithValue("@UrlId", urlId);
			cmd.Parameters.AddWithValue("@EventLink", eventLink);
			cmd.Parameters.AddWithValue("@Hash", hash);

			_dbConn.ExecuteNonQuery(cmd);
		}

		public bool RssSubscriptionRequiresProcessing(int eventRssSubscriptionId)
		{
			SqlCommand cmd = _dbConn.GetStoredProcedureCommand("EventRssSubscription_CheckRequiresProcessing");

			cmd.Parameters.AddWithValue("@EventRssSubscriptionId", eventRssSubscriptionId);

			SqlParameter outParam = cmd.Parameters.Add("@RequiresProcessing", SqlDbType.Bit);
			outParam.Direction = ParameterDirection.Output;

			_dbConn.ExecuteNonQuery(cmd);

			return (bool)outParam.Value;
		}

		public int? GetNextRssSubscriptionId()
		{
			SqlCommand cmd = _dbConn.GetStoredProcedureCommand("EventRssSubscription_GetNextRssSubscriptionId");
			DataSet ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0] == DBNull.Value)
				return null;

			return (int?)ds.Tables[0].Rows[0][0];
		}

		public DataTable Report_SummaryView(int siteId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Report_SummaryView");
			cmd.Parameters.AddWithValue("@SiteId", siteId);

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		public DataTable Report_RawDataView(int? configuredDatasourceId, int? datasourceSubTypeId, DateTime? start, DateTime? end)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Report_DetailView");

			cmd.Parameters.AddWithValue("@ConfiguredDatasourceId", configuredDatasourceId);
			cmd.Parameters.AddWithValue("@DatasourceSubTypeId", datasourceSubTypeId);
			cmd.Parameters.AddWithValue("@Start", start).DbType = DbType.DateTime;
			cmd.Parameters.AddWithValue("@End", end).DbType = DbType.DateTime;

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		#region Control Panel

		/// <summary>
		///		Gets a list of bit statuses that determine if certain items on the control
		///		panel should have exclamation points by them, because they need to be completed.
		/// </summary>
		/// <returns></returns>
		public DataTable ControlPanel_GetItemExclamations(Guid userId)
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("ControlPanel_GetItemExclamations");
			cmd.Parameters.AddWithValue("@UserId", userId);
			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		#endregion

		#region Administrative Reports

		public DataSet GetAdministrativeReport()
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Admin_GetAdministratorStatusReport");
			return _dbConn.GetDataSet(cmd);
		}

		public DataSet GetUserStatusReport(object userId)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Users_GetUserStatusReport");
			cmd.Parameters.AddWithValue("@Userid", userId);

			return _dbConn.GetDataSet(cmd);
		}

		#endregion

		#region Promotions
		public int? ValidatePromotion(string promoCode)
		{
			SqlCommand cmd = _dbConn.GetStoredProcedureCommand("Promo_ValidatePromotion");
			DataSet ds;

			cmd.Parameters.AddWithValue("@PromoCode", promoCode);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Columns.Count == 0)
				return null;
			else
			{
				object id = ds.Tables[0].Rows[0][0];
				if (id == DBNull.Value)
					return null;
				else
					return (int)id;
			}
		}

		public int? ValidatePromotion(string promoCode, Guid userId)
		{
			SqlCommand cmd = _dbConn.GetStoredProcedureCommand("Promo_ValidatePromotion");
			DataSet ds;

			cmd.Parameters.AddWithValue("@PromoCode", promoCode);
			cmd.Parameters.AddWithValue("@UserId", userId);

			ds = _dbConn.GetDataSet(cmd);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Columns.Count == 0)
				return null;
			else
			{
				object id = ds.Tables[0].Rows[0][0];
				if (id == DBNull.Value)
					return null;
				else
					return (int)id;
			}
		}
		#endregion

#region Saved Reports

		/// <summary>
		///		Deletes a saved report, and removes the saved report from any
		///		custom reports that it belongs to.
		/// </summary>
		/// <param name="savedReportId"></param>
		public void SavedReports_Delete(int savedReportId)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("SavedReports_Delete");
			cmd.Parameters.AddWithValue("@SavedReportId", savedReportId);
			_dbConn.ExecuteNonQuery(cmd);
		}

#endregion

		#region Keywords

		/// <summary>
		///		Gets a list of keywords for a particular site and search engine.
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="searchEngineId"></param>
		/// <returns></returns>
		public DataTable Keywords_GetKeywordList(int siteId, int searchEngineId)
		{
			DataSet ds;
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Keywords_GetKeywordList");
			cmd.Parameters.AddWithValue("@SiteId", siteId);
			cmd.Parameters.AddWithValue("@DatasourceTypeId", searchEngineId);

			ds = _dbConn.GetDataSet(cmd);

			return ds.Tables[0];
		}

		public void Keywords_BulkImport(int siteId, string[] keywords, int datasourceTypeId)
		{
			StringBuilder xmlKeywords;
			SqlCommand cmd;

			//Convert the keyword array to XML
			//Saimple: <keywords><keyword phrase="superjason"></keyword><keyword phrase="jason young"></keyword></keywords>
			xmlKeywords = new StringBuilder();
			xmlKeywords.Append("<keywords>");
			foreach (string keyword in keywords)
				xmlKeywords.AppendFormat("<keyword phrase=\"{0}\" />", HttpUtility.HtmlEncode(keyword));
			xmlKeywords.Append("</keywords>");

			cmd = _dbConn.GetStoredProcedureCommand("Keywords_BulkImport");
			cmd.Parameters.AddWithValue("@UrlId", siteId);
			cmd.Parameters.AddWithValue("@Keywords", xmlKeywords.ToString());
			cmd.Parameters.AddWithValue("@DatasourceTypeId", datasourceTypeId);

			_dbConn.ExecuteNonQuery(cmd);
		}

		#endregion

		#region Leads

		/// <summary>
		///		Records that a lead has been used, so it's usage data should
		///		be updated in the database.
		/// </summary>
		/// <param name="leadId"></param>
		public void Leads_IncrementLeadHit(string leadKey)
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("Leads_IncrementLeadHit");
			cmd.Parameters.AddWithValue("@LeadKey", leadKey);

			_dbConn.ExecuteNonQuery(cmd);
		}

		public DataTable Leads_LoadKeyMap()
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Leads_LoadKeyMap");

			ds = _dbConn.GetDataSet(cmd);
			return ds.Tables[0];
		}

		public DataTable Leads_LeadEffectiveness()
		{
			SqlCommand cmd;
			DataSet ds;

			cmd = _dbConn.GetStoredProcedureCommand("Leads_LeadEffectiveness");

			ds = _dbConn.GetDataSet(cmd);
			return ds.Tables[0];
		}

		#endregion
		
		#region Data Integrity Engine

		public void DataIntegrityEngine_RunCycle()
		{
			SqlCommand cmd;

			cmd = _dbConn.GetStoredProcedureCommand("DataIntegrityEngine_RunCycle");

			_dbConn.ExecuteNonQuery(cmd);
		}

		#endregion
	}
}
