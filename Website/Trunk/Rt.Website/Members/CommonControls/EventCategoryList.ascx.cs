using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Web;
using Rt.Website;

public partial class Members_CommonControls_EventCategoryList : UserControl
{
	public const string ANY_VALUE = "Any";

	private Database _db;

	/// <summary>
	///		Expose this for JavaScript access
	/// </summary>
	public DropDownList CategoryDropDownList
	{
		get { return ddlCategory; }
	}

	public bool AutoPostBack
	{
		get { return ddlCategory.AutoPostBack; }
		set { ddlCategory.AutoPostBack = value; }
	}

	public bool Enabled
	{
		get { return ddlCategory.Enabled; }
		set { ddlCategory.Enabled = value; }
	}

	public event EventHandler SelectedCategoryChanged;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (_db == null)
			_db = Global.GetDbConnection();

		ddlCategory.SelectedIndexChanged += ddlCategory_SelectedIndexChanged;
	}

	private void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
	{
		EventHandler eh;

		eh = SelectedCategoryChanged;
		if (eh != null)
			SelectedCategoryChanged(sender, e);
	}

	public void PopulateCategoryList(int siteId)
	{
		ICriteria criteria;
		IList<EventCategory> categories;

		_db = Global.GetDbConnection();

		criteria = _db.ORManager.Session.CreateCriteria(typeof (EventCategory)).Add(Expression.Eq("Url.Id", siteId));
		categories = criteria.List<EventCategory>();

		ddlCategory.DataSource = categories;
		ddlCategory.DataTextField = "Name";
		ddlCategory.DataValueField = "Id";
		ddlCategory.DataBind();

		//When loaded with AJAX, this item is added by the client
		ddlCategory.Items.Insert(0, new ListItem("<Default Category>", ANY_VALUE));
	}

	public List<EventCategoryJSON> GetEventCategoryList(string siteIdString)
	{
		List<EventCategoryJSON> categoriesJSON;
		IList<EventCategory> categories;
		ICriteria criteria;
		int siteId;

		if (string.IsNullOrEmpty(siteIdString))
			return new List<EventCategoryJSON>();

		siteId = int.Parse(siteIdString);

		_db = Global.GetDbConnection();

		criteria = _db.ORManager.Session.CreateCriteria(typeof (EventCategory)).Add(Expression.Eq("Url.Id", siteId));
		categories = criteria.List<EventCategory>();

		categoriesJSON = new List<EventCategoryJSON>();
		foreach (EventCategory currCategory in categories)
		{
			EventCategoryJSON item;

			item = new EventCategoryJSON();
			item.Text = currCategory.Name;
			item.Value = currCategory.Id.ToString();

			categoriesJSON.Add(item);
		}

		return categoriesJSON;
	}

	public void PopulateCategoryList(Members_CommonControls_SiteList siteList)
	{
		int? selectedSiteId;

		selectedSiteId = siteList.GetSelectedSiteId();
		if (selectedSiteId == null)
		{
			ddlCategory.Items.Clear();
			return;
		}

		PopulateCategoryList((int) selectedSiteId);
	}

	public int? GetSelectedCategoryId()
	{
		int selectedId;

		if (ddlCategory.SelectedIndex == -1 || ddlCategory.SelectedValue == ANY_VALUE)
			return null;

		selectedId = int.Parse(ddlCategory.SelectedValue);

		return selectedId;
	}

	public EventCategory GetSelectedCategory()
	{
		int? selectedId;

		selectedId = GetSelectedCategoryId();
		if (selectedId == null)
			return null;

		return _db.ORManager.Get<EventCategory>((int) selectedId);
	}

	public void SetSelectedCategory(EventCategory category)
	{
		if (category == null)
		{
			SelectAnyItem();
			return;
		}

		ddlCategory.SelectedValue = category.Id.ToString();
	}

	public void SelectAnyItem()
	{
		if (ddlCategory.Items.Count > 0)
			ddlCategory.SelectedIndex = 0;
	}
}