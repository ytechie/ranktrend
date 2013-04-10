using System;

namespace Rt.Framework.Components
{
	/// <summary>
	///		Represents a parameter for a datasource.  This is not what is used
	///		to store parameter values, but defines a parameter value.
	/// </summary>
	[Serializable]
	public class DatasourceParameterType
	{
		private DatasourceType _datasourceType;
		private string _description;
		private int _id;
		private bool _masked;
		private int _parameterNumber;
		private bool _showInName;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public DatasourceType DatasourceType
		{
			get { return _datasourceType; }
			set { _datasourceType = value; }
		}

		public int ParameterNumber
		{
			get { return _parameterNumber; }
			set { _parameterNumber = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public bool Masked
		{
			get { return _masked; }
			set { _masked = value; }
		}

		public bool ShowInName
		{
			get { return _showInName; }
			set { _showInName = value; }
		}

		/// <summary>
		///		Generates the encryption key based on the user, and a base key.
		/// </summary>
		/// <returns></returns>
		public static string GetEncryptionKey(Guid userId)
		{
			const string ENCRYPTION_BASE = "as3$TAJla;aj";

			return ENCRYPTION_BASE + userId.ToString();
		}
	}
}