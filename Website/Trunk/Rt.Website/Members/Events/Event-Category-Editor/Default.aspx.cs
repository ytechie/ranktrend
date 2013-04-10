using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web.JavaScript;

public partial class Members_Events_Event_Category_Editor_Default : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		siteList.SelectedItemChanged += siteList_SelectedItemChanged;

		lstCategories.SelectedIndexChanged += lstCategories_SelectedIndexChanged;
		cmdAdd.Click += cmdAdd_Click;
		cmdDelete.Click += cmdDelete_Click;

		cmdSave.Click += cmdSave_Click;

		if (!Page.IsPostBack)
		{
			initDeleteConfirm();

			siteList.PopulatePageList();
			populateCategoryList();
		}
	}

	private void initDeleteConfirm()
	{
		string msg;

		msg =
			"Are you sure you want to PERMANENTLY delete this category and all associated events? If any event subscriptions are using this category, they will be reassigned to the default category";

		JavaScriptBlock.ConfirmClick(cmdDelete, msg);
	}

	private void siteList_SelectedItemChanged(object sender, EventArgs e)
	{
		populateCategoryList();
	}

	#region Category List

	private void populateCategoryList()
	{
		ICriteria criteria;
		IList<EventCategory> categories;
		int? urlId;

		urlId = siteList.GetSelectedSiteId();
		if (urlId == null)
			return;

		criteria = _db.ORManager.Session.CreateCriteria(typeof (EventCategory)).Add(Expression.Eq("Url.Id", urlId));
		categories = criteria.List<EventCategory>();

		lstCategories.DataSource = categories;
		lstCategories.DataTextField = "Name";
		lstCategories.DataValueField = "Id";
		lstCategories.DataBind();
	}

	private EventCategory getSelectedCategory()
	{
		EventCategory selectedCategory;
		int categoryId;

		if (lstCategories.SelectedIndex == -1)
			return null;

		categoryId = int.Parse(lstCategories.SelectedValue);
		selectedCategory = _db.ORManager.Get<EventCategory>(categoryId);

		return selectedCategory;
	}

	private void lstCategories_SelectedIndexChanged(object sender, EventArgs e)
	{
		displayCategory(getSelectedCategory());
	}

	private void cmdAdd_Click(object sender, EventArgs e)
	{
		EventCategory newCategory;

		newCategory = new EventCategory();
		newCategory.Name = "Untitled Category";
		newCategory.Url = siteList.GetSelectedSite();

		_db.ORManager.SaveOrUpdate(newCategory);

		lstCategories.Items.Add(new ListItem(newCategory.Name, newCategory.Id.ToString()));

		//Select the new item
		lstCategories.SelectedIndex = lstCategories.Items.Count - 1;
		displayCategory(newCategory);
	}

	private void cmdDelete_Click(object sender, EventArgs e)
	{
		EventCategory selectedCategory;
		Database db;

		selectedCategory = getSelectedCategory();

		db = Global.GetDbConnection();

		db.DeleteEventCategory(selectedCategory.Id);

		lstCategories.Items.Remove(lstCategories.SelectedItem);
		displayCategory(null);
	}

	#endregion

	#region Details

	private void displayCategory(EventCategory category)
	{
		if (category == null)
		{
			txtName.Text = "";
			txtName.Enabled = false;
			cmdSave.Enabled = false;
		}
		else
		{
			txtName.Text = category.Name;
			txtName.Enabled = true;
			cmdSave.Enabled = true;
		}
	}

	private void cmdSave_Click(object sender, EventArgs e)
	{
		EventCategory selectedCategory;

		selectedCategory = getSelectedCategory();
		selectedCategory.Name = txtName.Text;

		//Updated the display name in the category list
		lstCategories.SelectedItem.Text = selectedCategory.Name;

		_db.ORManager.SaveOrUpdate(selectedCategory);
	}

	#endregion
}