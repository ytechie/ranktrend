using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Rt.Framework.Applications.InteractiveReport;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using UNLV.IAP.WebControls;

public partial class Members_Interactive_Report_DatasourceTable : UserControl
{
	/// <summary>
	///		Declare and create our logger for this class.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Database _db;
	private bool _listPopulated;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		initialize();

		if (!Page.IsPostBack)
		{
			PopulateDatasources();
		}
	}

	private void initialize()
	{
		if (_userId == Guid.Empty)
			_userId = (Guid) Membership.GetUser().ProviderUserKey;
		if (_db == null)
			_db = Global.GetDbConnection();
	}

	public void PopulateDatasources()
	{
		DataTable dsList;

		initialize();

		if (_listPopulated)
			return;

		dsList = _db.GetTrendDatasoureList(_userId);
		_log.DebugFormat("Retrieved {0} datasources to dispay for user {1}", dsList.Rows.Count, _userId);

		dgDatasources.DataSource = dsList;
		dgDatasources.DataBind();

		//Don't populate the list twice
		_listPopulated = true;
	}

	public DisplayDatasourceItem[] GetSelectedDatasources()
	{
		List<DisplayDatasourceItem> _selectedList;
		DisplayDatasourceItem currDatasourceItem;

		//Controls in the grid
		CheckBox currCheckbox;
		HtmlColorDropDown colorDropDown;
		DropDownList currDropDownList;

		_selectedList = new List<DisplayDatasourceItem>();

		foreach (DataGridItem currItem in dgDatasources.Items)
		{
			currDatasourceItem = new DisplayDatasourceItem();

			//Get the ID's
			currDatasourceItem.ConfiguredDatasourceId = grid_GetDatasourceId(currItem);
			currDatasourceItem.DatasourceSubTypeId = grid_GetDatasourceSubTypeId(currItem);

			//Get the values from the other controls
			currCheckbox = grid_GetRawCheckBox(currItem);
			currDatasourceItem.ShowRaw = currCheckbox.Checked;
			currCheckbox = grid_GetTrendLineCheckBox(currItem);
			currDatasourceItem.ShowTrendLine = currCheckbox.Checked;
			currCheckbox = grid_GetCurveFitCheckBox(currItem);
			currDatasourceItem.ShowLowess = currCheckbox.Checked;

			colorDropDown = grid_GetColorDropDown(currItem);
			if (colorDropDown.SelectedColor.ToArgb() == 0)
				currDatasourceItem.Color = null;
			else
				currDatasourceItem.Color = colorDropDown.SelectedColor;

			currDropDownList = grid_GetLineThicknessDropDown(currItem);
			currDatasourceItem.LineThickness = int.Parse(currDropDownList.SelectedValue);

			//Only add the item if the user chose to display any of them
			if (currDatasourceItem.ShowRaw || currDatasourceItem.ShowTrendLine || currDatasourceItem.ShowLowess)
				_selectedList.Add(currDatasourceItem);
		}

		return _selectedList.ToArray();
	}

	public void SetSelectedDatasources(DisplayDatasourceItem[] datasourceItems)
	{
		int currDatasourceId;
		int? currSubTypeId;
		DisplayDatasourceItem displayItem;

		foreach (DataGridItem currItem in dgDatasources.Items)
		{
			currDatasourceId = grid_GetDatasourceId(currItem);
			currSubTypeId = grid_GetDatasourceSubTypeId(currItem);

			//Set this to null so we know if we found an item to display
			displayItem = null;

			//I know this will loop a lot, but in actuality, it won't be bad
			foreach (DisplayDatasourceItem currDisplayItem in datasourceItems)
			{
				if (currDisplayItem.ConfiguredDatasourceId == currDatasourceId &&
				    currDisplayItem.DatasourceSubTypeId == currSubTypeId)
				{
					//We found a matching item
					displayItem = currDisplayItem;
					break;
				}
			}

			if (displayItem == null)
			{
				//There was no matching item to load, so set the defaults
				displayItem = new DisplayDatasourceItem();
				displayItem.LineThickness = 1;
				displayItem.ShowRaw = false;
				displayItem.ShowTrendLine = false;
				displayItem.ShowLowess = false;
			}

			//Set the current row to the item values
			grid_GetRawCheckBox(currItem).Checked = displayItem.ShowRaw;
			grid_GetTrendLineCheckBox(currItem).Checked = displayItem.ShowTrendLine;
			grid_GetCurveFitCheckBox(currItem).Checked = displayItem.ShowLowess;
			grid_GetLineThicknessDropDown(currItem).SelectedValue = displayItem.LineThickness.ToString();

			//TODO:Set the color
		}
	}

	#region Grid Control Accessors

	private int grid_GetDatasourceId(DataGridItem gridRow)
	{
		return int.Parse(gridRow.Cells[(int) DSGridColumns.Id].Text);
	}

	private int? grid_GetDatasourceSubTypeId(DataGridItem gridRow)
	{
		string cellText;
		int? subTypeId;

		cellText = gridRow.Cells[(int) DSGridColumns.SubTypeId].Text;
		if (cellText == "&nbsp;")
			subTypeId = null;
		else
			subTypeId = int.Parse(cellText);

		return subTypeId;
	}

	private CheckBox grid_GetRawCheckBox(DataGridItem gridRow)
	{
		return gridRow.Cells[(int) DSGridColumns.Selected].FindControl("chkSelected") as CheckBox;
	}

	private CheckBox grid_GetTrendLineCheckBox(DataGridItem gridRow)
	{
		return gridRow.Cells[(int) DSGridColumns.TrendLine].FindControl("chkTrendLine") as CheckBox;
	}

	private CheckBox grid_GetCurveFitCheckBox(DataGridItem gridRow)
	{
		return gridRow.Cells[(int) DSGridColumns.CurveFit].FindControl("chkCurveFit") as CheckBox;
	}

	private HtmlColorDropDown grid_GetColorDropDown(DataGridItem gridRow)
	{
		return gridRow.Cells[(int) DSGridColumns.Color].FindControl("ddlColor") as HtmlColorDropDown;
	}

	private DropDownList grid_GetLineThicknessDropDown(DataGridItem gridRow)
	{
		return gridRow.Cells[(int) DSGridColumns.Thickness].FindControl("ddlLineThickness") as DropDownList;
	}

	#endregion

	#region Nested type: DSGridColumns

	private enum DSGridColumns
	{
		Id,
		SubTypeId,
		Selected,
		DataTypeName,
		PageName,
		TrendLine,
		CurveFit,
		Color,
		Thickness
	}

	#endregion
}