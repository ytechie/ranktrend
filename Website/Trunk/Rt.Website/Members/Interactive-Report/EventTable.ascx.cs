using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rt.Framework.Applications.InteractiveReport;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Interactive_Report_EventTable : UserControl
{
	private Database _db;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();
		_userId = (Guid) Membership.GetUser().ProviderUserKey;

		if (!Page.IsPostBack)
			populateCategoryList();
	}

	private void populateCategoryList()
	{
		DataTable dt;

		dt = _db.IR_GetEventCategoryList(_userId);

		dgEvents.DataSource = dt;
		dgEvents.DataBind();
	}

	public DisplayEventCategoryItem[] GetSelectedEventCategories()
	{
		List<DisplayEventCategoryItem> _selectedList;
		DisplayEventCategoryItem currEventCategory;

		//Controls in the grid
		CheckBox currCheckbox;

		_selectedList = new List<DisplayEventCategoryItem>();

		foreach (DataGridItem currItem in dgEvents.Items)
		{
			currEventCategory = new DisplayEventCategoryItem();

			//Get the ID's from the hidden columns
			currEventCategory.EventCategoryId = grid_GetEventCategoryId(currItem);

			//Get the values from the other controls
			currCheckbox = grid_GetSelectedCheckbox(currItem);

			if (currCheckbox.Checked)
				_selectedList.Add(currEventCategory);
		}

		return _selectedList.ToArray();
	}

	private int grid_GetEventCategoryId(DataGridItem currItem)
	{
		return int.Parse(currItem.Cells[(int) GridColumns.Id].Text);
	}

	private CheckBox grid_GetSelectedCheckbox(DataGridItem currItem)
	{
		return currItem.Cells[(int) GridColumns.Selected].FindControl("chkSelected") as CheckBox;
	}

	public void SetSelectedEventCategories(DisplayEventCategoryItem[] eventCategoryItems)
	{
		int eventCategoryId;
		CheckBox selectedCheckbox;

		foreach (DataGridItem currItem in dgEvents.Items)
		{
			//Get the ID's from the hidden columns
			eventCategoryId = grid_GetEventCategoryId(currItem);
			selectedCheckbox = grid_GetSelectedCheckbox(currItem);

			//Uncheck the item unless it's in the settings
			selectedCheckbox.Checked = false;

			foreach (DisplayEventCategoryItem currDisplayItem in eventCategoryItems)
			{
				if (currDisplayItem.EventCategoryId == eventCategoryId)
				{
					//We found a matching item
					selectedCheckbox.Checked = true;
					break;
				}
			}
		}
	}

	#region Nested type: GridColumns

	private enum GridColumns
	{
		Id,
		Selected,
		Name,
		Url
	}

	#endregion
}