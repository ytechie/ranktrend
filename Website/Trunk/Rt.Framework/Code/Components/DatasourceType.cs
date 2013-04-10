using System;
using System.Collections.Generic;

namespace Rt.Framework.Components
{
	[Serializable]
	public class DatasourceType
	{
		private IList<DatasourceSubType> _datasourceSubTypes;
		private string _description;
		private bool _enabled;
		private int _id;
		private bool _reversed;
		private SearchEngine _searchEngine;

		/// <summary>
		///		The unique identifier of this datasource type
		/// </summary>
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		Gets or sets a description of this datasource
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public SearchEngine SearchEngine
		{
			get { return _searchEngine; }
			set { _searchEngine = value; }
		}

		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		public bool Reversed
		{
			get { return _reversed; }
			set { _reversed = value; }
		}

		public IList<DatasourceSubType> DatasourceSubTypes
		{
			get { return _datasourceSubTypes; }
			set { _datasourceSubTypes = value; }
		}
	}
}