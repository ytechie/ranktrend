using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_CommonControls_SiteList : UserControl
{
	private const string COOKIE_SAVED_SELECTION = "sss";

	/// <summary>
	///		Create and declare our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Database _db;
	private bool _listPopulated;

	/// <summary>
	///		Expose this for JavaScript access
	/// </summary>
	public DropDownList SiteDropDownList
	{
		get { return ddlSites; }
	}

	public bool AutoPostBack
	{
		get { return ddlSites.AutoPostBack; }
		set { ddlSites.AutoPostBack = value; }
	}

	public event EventHandler SelectedItemChanged;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack && _listPopulated == false)
			PopulatePageList();

		ddlSites.SelectedIndexChanged += ddlSites_SelectedIndexChanged;
	}

	private void ddlSites_SelectedIndexChanged(object sender, EventArgs e)
	{
		EventHandler eh;

		saveSelection();

		eh = SelectedItemChanged;
		if (eh != null)
			eh(sender, e);
	}

	public void PopulatePageList()
	{
		ICriteria criteria;
		IList<UrlClass> pages;
		Guid userId;

		userId = (Guid) Membership.GetUser().ProviderUserKey;
		_db = Global.GetDbConnection();

		//Try to get the cached list first
		pages = SessionCache.GetSiteList(Session);

		if (pages == null)
		{
			criteria = _db.ORManager.Session.CreateCriteria(typeof (UrlClass)).Add(Expression.Eq("UserId", userId));
			pages = criteria.List<UrlClass>();

			//Cache the latest page list
			SessionCache.SaveSiteList(Session, pages);
			_log.Debug("Saving the site list to the session cache");
		}
		else
		{
			_log.Debug("Loaded site list from the session cache");
		}

		ddlSites.DataSource = pages;
		ddlSites.DataTextField = "Url";
		ddlSites.DataValueField = "Id";
		ddlSites.DataBind();

		_listPopulated = true;

		restoreSelection();
	}

	private void restoreSelection()
	{
		HttpCookie cookie;

		cookie = Request.Cookies[COOKIE_SAVED_SELECTION];
		if (cookie == null)
			return;

		if (cookie.Value != null)
		{
			if (ddlSites.Items.FindByValue(cookie.Value) != null)
				ddlSites.SelectedValue = cookie.Value;
		}
	}

	private void saveSelection()
	{
		if (ddlSites.SelectedIndex != -1)
		{
			HttpCookie cookie;

			cookie = new HttpCookie(COOKIE_SAVED_SELECTION, ddlSites.SelectedValue);
			Response.Cookies.Add(cookie);
		}
	}

	public int? GetSelectedSiteId()
	{
		int selectedSiteId;

		if (ddlSites.SelectedIndex == -1)
			return null;

		selectedSiteId = int.Parse(ddlSites.SelectedValue);

		return selectedSiteId;
	}

	public UrlClass GetSelectedSite()
	{
		int? selectedSiteId;
		UrlClass selectedSite = null;
		IList<UrlClass> pages;

		selectedSiteId = GetSelectedSiteId();

		if (selectedSiteId == null)
			return null;

		//Try to load the site from the cache
		pages = SessionCache.GetSiteList(Session);
		if (pages != null)
		{
			foreach (UrlClass currPage in pages)
			{
				if (currPage.Id == selectedSiteId)
				{
					selectedSite = currPage;
					break;
				}
			}
		}

		//If the site isn't cached, load the site
		if (selectedSite == null)
		{
			_log.Debug("Couldn't find the site in the cache");
			selectedSite = _db.ORManager.Get<UrlClass>(selectedSiteId);
		}
		else
		{
			_log.Debug("The site was found in the cache, no need to load from the database");
		}

		return selectedSite;
	}

	public void SetSelectedSiteId(int siteId)
	{
		ddlSites.SelectedValue = siteId.ToString();
		saveSelection();
	}
}