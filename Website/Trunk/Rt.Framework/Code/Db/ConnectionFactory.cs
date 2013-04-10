using System;
using System.Reflection;
using System.Text;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using YTech.Db;
using DbConnection=YTech.Db.SqlServer.DbConnection;

namespace Rt.Framework.Db
{
	/// <summary>
	///		Provides an easy way to create database connections to the
	///		Rank Trend database
	/// </summary>
	public class ConnectionFactory
	{
		private static ORManager _orManager;
		private static object _orManagerLocker = new object();

		private ConnectionFactory()
		{
		}

		/// <summary>
		///		Creates and prepares a <see cref="Database"/> object that
		///		is ready to interact with the specified database.
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use the connection string from the web.config instead")]
		public static Database GetDbConnection(string databaseServerName, string databaseName, string userName,
		                                       string password)
		{
			StringBuilder sb;

			sb = new StringBuilder();

			sb.AppendFormat("Data Source={0};", databaseServerName);
			sb.AppendFormat("Initial Catalog={0};", databaseName);
			sb.AppendFormat("User ID={0};", userName);
			sb.AppendFormat("Password={0};", password);

			return GetDbConnectionFromConnectionString(sb.ToString());
		}

		/// <summary>
		///		Creates and prepares a <see cref="Database"/> object that
		///		is ready to interact with the specified database.
		/// </summary>
		/// <remarks>
		///		WARNING: There is no support for multiple NHibernate connections.  The connection
		///		string that is used first will be the one that is used to create the NHibernate
		///		session factory.
		/// </remarks>
		/// <param name="connectionString">
		///		The full connection string that can be used to connect to
		///		the database.
		///	</param>
		public static Database GetDbConnectionFromConnectionString(string connectionString)
		{
			Database db;
			DbConnection dbConn;

			lock (_orManagerLocker)
			{
				if (_orManager == null)
					_orManager = new ORManager(connectionString, new Assembly[] {typeof (ConfiguredDatasource).Assembly});
			}

			dbConn = new DbConnection(connectionString);
			db = new Database(dbConn, _orManager);

			return db;
		}

		public static ORManager GetORManager(string connectionString)
		{
			return new ORManager(connectionString, new Assembly[] { typeof(ConfiguredDatasource).Assembly });
		}
	}
}