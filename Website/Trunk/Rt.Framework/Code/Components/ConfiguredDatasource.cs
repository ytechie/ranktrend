using System;
using System.Collections.Generic;
using NHibernate.Collection.Generic;
using Rt.Framework.Services.DataEngine;
using YTech.General.Cryptography;

namespace Rt.Framework.Components
{
	/// <summary>
	///		Represents a datasource that has been configured to run.
	/// </summary>
	/// <remarks>
	///		This could be considered an "instance" of a datasource"
	/// </remarks>
	[Serializable]
	public class ConfiguredDatasource
	{
		private TimeSpan _checkFrequency;
		private DatasourceType _datasourceType;
		private string _description;
		private bool _enabled;
		private int? _id;
		private DateTime? _lastCheckAttempt;
		private string _name;
		//This is needed because NHibernate can't set an IList yet!
		private PersistentGenericSet<DatasourceParameter> _parameters;
		private UrlClass _url;

		/// <summary>
		///		The unique database ID for this particular configured datasource.
		/// </summary>
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		The Unique identifier of the URL that this
		///		datasource is configured for.
		/// </summary>
		public UrlClass Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public DatasourceType DatasourceType
		{
			get { return _datasourceType; }
			set { _datasourceType = value; }
		}

		/// <summary>
		///		The amount of days between each check of the
		///		datasource.
		/// </summary>
		public int CheckFrequencyDays
		{
			get { return (int) _checkFrequency.TotalDays; }
			set { _checkFrequency = TimeSpan.FromDays(value); }
		}

		/// <summary>
		///		The amount of time between each check of the datasource.
		/// </summary>
		public TimeSpan CheckFrequency
		{
			get { return _checkFrequency; }
			set { _checkFrequency = value; }
		}

		/// <summary>
		///		Gets or sets a value indicating if this configured data
		///		source is enabled, meaning new data will be retreived for it.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		public DateTime? LastCheckAttempt
		{
			get { return _lastCheckAttempt; }
			set { _lastCheckAttempt = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public PersistentGenericSet<DatasourceParameter> Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		public string DisplayNameWithUrl
		{
			get
			{
				if (_url == null)
					return _name;

				return string.Format("{0} ({1})", _name, _url.Url);
			}
		}

		/// <summary>
		///		Prepares a datasource for execution by setting the URL,
		///		and the parameter list.
		/// </summary>
		/// <param name="datasource"></param>
		/// <param name="parameterTypes"></param>
		public void InitializeDatasource(IDataSource datasource, DatasourceParameterType[] parameterTypes)
		{
			object parameterValue;
			DatasourceParameterType parameterType;
			string encryptionKey;

			encryptionKey = DatasourceParameterType.GetEncryptionKey(Url.UserId);

			//Set the properties that are required for the datasource to work
			datasource.Url = Url.Url;
			datasource.Parameters = new Dictionary<int, object>();
			foreach (DatasourceParameter dp in _parameters)
			{
				parameterType = null;

				//Find the correct type
				foreach (DatasourceParameterType currType in parameterTypes)
				{
					if (currType.ParameterNumber == dp.ParameterNumber)
					{
						parameterType = currType;
						break;
					}
				}

				//Check if the parameter requires decryption
				if (parameterType != null && parameterType.Masked && dp.Value != null)
					parameterValue = SymmetricEncryption.DecryptData(encryptionKey, dp.Value.ToString());
				else
					parameterValue = dp.Value;

				datasource.Parameters.Add((int) dp.ParameterNumber, parameterValue);
			}
		}
	}
}