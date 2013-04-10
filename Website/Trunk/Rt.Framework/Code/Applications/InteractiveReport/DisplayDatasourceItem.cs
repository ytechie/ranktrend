using System;
using System.Drawing;

namespace Rt.Framework.Applications.InteractiveReport
{
	/// <summary>
	///		This class represents a datasource item that is displayed in a list for
	///		a user to select the data they want to display.
	/// </summary>
	[Serializable]
	public class DisplayDatasourceItem
	{
		private Color? _color;
		private int _configuredDatasourceId;
		private int? _datasourceSubTypeId;
		private int _lineThickness;
		private bool _showLowess;
		private bool _showRaw;
		private bool _showTrendLine;

		public int ConfiguredDatasourceId
		{
			get { return _configuredDatasourceId; }
			set { _configuredDatasourceId = value; }
		}

		public int? DatasourceSubTypeId
		{
			get { return _datasourceSubTypeId; }
			set { _datasourceSubTypeId = value; }
		}

		public bool ShowRaw
		{
			get { return _showRaw; }
			set { _showRaw = value; }
		}

		public bool ShowTrendLine
		{
			get { return _showTrendLine; }
			set { _showTrendLine = value; }
		}

		public bool ShowLowess
		{
			get { return _showLowess; }
			set { _showLowess = value; }
		}

		public Color? Color
		{
			get { return _color; }
			set { _color = value; }
		}

		public int LineThickness
		{
			get { return _lineThickness; }
			set { _lineThickness = value; }
		}
	}
}