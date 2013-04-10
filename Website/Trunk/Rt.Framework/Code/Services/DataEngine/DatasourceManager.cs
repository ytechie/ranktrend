using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using log4net;

namespace Rt.Framework.Services.DataEngine
{
	/// <summary>
	///		Reads in all of the IDataSource classes from an assembly, and
	///		allows them to be retrieved by their type ID.
	/// </summary>
	public class DatasourceManager
	{
		private Dictionary<int, Type> _datasources;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public DatasourceManager()
		{
			Assembly asm;
			Type[] types;
			object[] attributes;
			DatasourceAttribute currDatasourceAttribute;
			
			asm = GetType().Assembly;
			types = asm.GetTypes();

			_datasources = new Dictionary<int, Type>();
			
			foreach (Type currType in types)
			{
				attributes = currType.GetCustomAttributes(false);

				if (attributes != null)
				{
					foreach (object currAttribute in attributes)
					{
						if (currAttribute.GetType() == typeof(DatasourceAttribute))
						{
							currDatasourceAttribute = (DatasourceAttribute)currAttribute;
							_datasources.Add(currDatasourceAttribute.UniqueId, currType);
						}
					}
				}
			}
		}

		public IDictionary<int, Type> GetAllDatasourceTypes()
		{
			return _datasources;
		}

		public IDataSource GetDatasource(int datasourceTypeId)
		{
			Type currType;

			//If a manager doesn't exist for the manager, return null
			if (!_datasources.ContainsKey(datasourceTypeId))
			{
				_log.WarnFormat("An IDataSource was requested for type {0}, but one could not be located", datasourceTypeId);
				return null;
			}

			currType = _datasources[datasourceTypeId];
			try
			{
				return (IDataSource)currType.Assembly.CreateInstance(currType.FullName);
			}
			catch (MissingMethodException mme)
			{
				_log.Warn("Unable to load one of the IDataSource types, due to the fact that it did not have a blank constructor", mme);
				return null;
			}
		}
	}
}
