using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Applications.Events;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Web;
using Rt.Website;
using YTech.General.Web.JSProxy;

public partial class Members_Events_Event_Editor_Default : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		AjaxServerMethodHandler.CheckAjaxCall(this);

		if (!Page.IsPostBack)
		{
			int? selectedSiteId;

			siteList.PopulatePageList();
			selectedSiteId = siteList.GetSelectedSiteId();
			if (selectedSiteId != null)
				categoryList.PopulateCategoryList((int) selectedSiteId);
			populateEventList();
		}

		Global.AddCommonJavaScript(this);
		Page.ClientScript.RegisterClientScriptInclude("jquery-calendar", ResolveUrl("~/Scripts/jquery-calendar.pack.js"));
		Page.ClientScript.RegisterClientScriptInclude("jquery.listbox", ResolveUrl("~/Scripts/jquery.listbox.js"));
		Page.ClientScript.RegisterClientScriptInclude("json", ResolveUrl("~/Scripts/json.js"));
		Page.ClientScript.RegisterClientScriptInclude("local", ResolveUrl("Scripts.js"));

		//Register some child controls for javascript
		((MasterPage) Master).ControlProxies.AddExtraControl("ddlSites", siteList.SiteDropDownList);
		((MasterPage) Master).ControlProxies.AddExtraControl("ddlCategories", categoryList.CategoryDropDownList);
	}

	private void populateEventList()
	{
		ICriteria criteria;
		IList<Event> events;
		int? selectedCategoryId;
		int? selectedPageId;

		lstEvents.Items.Clear();

		selectedPageId = siteList.GetSelectedSiteId();
		if (selectedPageId == null)
			return;

		selectedCategoryId = categoryList.GetSelectedCategoryId();

		criteria = _db.ORManager.Session.CreateCriteria(typeof (Event));
		criteria.Add(Expression.Eq("Url.Id", selectedPageId));
		if (selectedCategoryId == null)
			criteria.Add(Expression.IsNull("Category.Id"));
		else
			criteria.Add(Expression.Eq("Category.Id", selectedCategoryId));
		criteria.AddOrder(Order.Desc("StartTime"));

		events = criteria.List<Event>();

		lstEvents.Items.Clear();
		foreach (Event currEvent in events)
		{
			ListItem newItem;
			EventJSON eventJson;

			eventJson = getJsonEvent(currEvent);

			newItem = new ListItem();
			newItem.Text = currEvent.Name;
			newItem.Value = currEvent.Id.ToString();

			newItem.Attributes["eventJSON"] = JavaScriptConvert.SerializeObject(eventJson);

			lstEvents.Items.Add(newItem);
		}
	}

	private static EventJSON getJsonEvent(Event eventToConvert)
	{
		EventJSON json;

		json = new EventJSON();
		json.Id = eventToConvert.Id.ToString();
		if (eventToConvert.Color != null)
			json.ColorName = getNamedColorFromColor(eventToConvert.Color.Value).Name;
		json.Description = eventToConvert.Description;
		json.StartString = eventToConvert.StartTime.ToString("d");
		if (eventToConvert.EndTime != null)
			json.EndString = eventToConvert.EndTime.Value.ToString("d");
		if (eventToConvert.Category != null)
			json.EventCategoryId = eventToConvert.Category.Id.ToString();
		json.Name = eventToConvert.Name;

		return json;
	}

	#region AJAX

	[JSProxy]
	public List<EventCategoryJSON> GetEventCategoryList(string siteIdString)
	{
		return categoryList.GetEventCategoryList(siteIdString);
	}

	[JSProxy]
	public void Save(string jsonData)
	{
		EventJSON eventData;
		Event e;

		eventData = (EventJSON) JavaScriptConvert.DeserializeObject(jsonData, typeof (EventJSON));

		e = _db.ORManager.Get<Event>(int.Parse(eventData.Id));
		e.Name = eventData.Name;
		e.Description = eventData.Description;
		e.StartTime = DateTime.Parse(eventData.StartString);
		if (eventData.EndString == null || eventData.EndString.Length == 0)
			e.EndTime = null;
		else
			e.EndTime = DateTime.Parse(eventData.EndString);
		if (!string.IsNullOrEmpty(eventData.ColorName))
			e.Color = getColorFromName(eventData.ColorName);

		_db.ORManager.SaveOrUpdate(e);
	}

	[JSProxy]
	public EventJSON AddEvent(string selectedUrlId, string selectedCategoryId)
	{
		Event newEvent;
		UrlClass url;
		EventCategory category;
		int siteId;

		siteId = int.Parse(selectedUrlId);

		url = _db.ORManager.Get<UrlClass>(siteId);
		if (selectedCategoryId == "Any")
			category = null;
		else
			category = _db.ORManager.Get<EventCategory>(int.Parse(selectedCategoryId));

		newEvent = new Event();
		newEvent.Name = "Untitled Event";
		newEvent.StartTime = DateTime.Now;
		newEvent.Url = url;
		newEvent.Category = category;

		_db.ORManager.SaveOrUpdate(newEvent);

		return getJsonEvent(newEvent);
	}

	[JSProxy]
	public void DeleteEvent(int eventId)
	{
		Event evt;

		evt = _db.ORManager.Get<Event>(eventId);
		_db.ORManager.Delete(evt);
	}

	[JSProxy]
	public IList<EventJSON> GetEventList(string siteIdString, string categoryIdString)
	{
		ICriteria criteria;
		IList<Event> events;
		IList<EventJSON> jsonEvents;
		int? categoryId;

		if (categoryIdString == "Any")
			categoryId = null;
		else
			categoryId = int.Parse(categoryIdString);

		criteria = _db.ORManager.Session.CreateCriteria(typeof (Event));
		criteria.Add(Expression.Eq("Url.Id", int.Parse(siteIdString)));
		if (categoryId == null)
			criteria.Add(Expression.IsNull("Category.Id"));
		else
			criteria.Add(Expression.Eq("Category.Id", categoryId));
		criteria.AddOrder(Order.Desc("StartTime"));

		events = criteria.List<Event>();

		jsonEvents = new List<EventJSON>(events.Count);
		foreach (Event currEvent in events)
			jsonEvents.Add(getJsonEvent(currEvent));

		return jsonEvents;
	}

	#endregion

	#region Color Utilities

	private static Color getNamedColorFromColor(Color c)
	{
		foreach (KnownColor colorEnum in Enum.GetValues(typeof (KnownColor)))
		{
			if (c.ToArgb() == Color.FromKnownColor(colorEnum).ToArgb())
				return Color.FromKnownColor(colorEnum);
		}

		return c;
	}

	private static Color getColorFromName(string colorName)
	{
		foreach (KnownColor colorEnum in Enum.GetValues(typeof (KnownColor)))
		{
			if (colorEnum.ToString() == colorName)
				return Color.FromKnownColor(colorEnum);
		}

		return Color.Empty;
	}

	#endregion
}