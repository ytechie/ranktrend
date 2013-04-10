using System;

namespace Rt.Framework.Components
{
	/// <summary>
	///		Represents a value that has been set for a datasource.
	/// </summary>
	[Serializable]
	public class DatasourceParameter
	{
		private ConfiguredDatasource _configuredDatasource;
		private int? _id;
		private int? _intValue;
		private int _parameterNumber;
		private string _textValue;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int ParameterNumber
		{
			get { return _parameterNumber; }
			set { _parameterNumber = value; }
		}

		public int? IntValue
		{
			get { return _intValue; }
			set { _intValue = value; }
		}

		public string TextValue
		{
			get { return _textValue; }
			set { _textValue = value; }
		}

		public ConfiguredDatasource ConfiguredDatasource
		{
			get { return _configuredDatasource; }
			set { _configuredDatasource = value; }
		}

		/// <summary>
		///		Gets an object that contains the text value or
		///		int value for this parameter.  The int value is
		///		used if the text value is null.
		/// </summary>
		public object Value
		{
			get
			{
				if (_textValue != null)
					return _textValue;
				else
					return _intValue;
			}
		}
	}
}