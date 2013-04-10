using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web.JavaScript;

public partial class Members_Site_Editor_Default : Page
{
	/// <summary>
	///		Create and declare our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Database _db;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_userId = (Guid) Membership.GetUser().ProviderUserKey;
		_db = Global.GetDbConnection();

		lstSites.SelectedIndexChanged += lstSites_SelectedIndexChanged;
		cmdAdd.Click += cmdAdd_Click;
		cmdDelete.Click += cmdDelete_Click;
		cmdSave.Click += cmdSave_Click;

		if (!Page.IsPostBack)
		{
			populateSiteList();
			displaySite(getSelectedSite());

			//Confirm the delete click
			JavaScriptBlock.ConfirmClick(cmdDelete, "Are you sure you want to delete this site and all data associated with it?");
		}
	}

	private void cmdDelete_Click(object sender, EventArgs e)
	{
		UrlClass selectedSite;
		IList<UrlClass> cachedSites;

		selectedSite = getSelectedSite();

		if (selectedSite != null)
		{
			_db.ORManager.Delete(selectedSite);
			lstSites.Items.Remove(lstSites.SelectedItem);

			if (lstSites.Items.Count > 0)
			{
				lstSites.SelectedIndex = lstSites.Items.Count - 1;
				displaySite(getSelectedSite());
			}

			//These will definitely be cached because we saved them when the list loaded
			cachedSites = SessionCache.GetSiteList(Session);
			cachedSites.Remove(selectedSite);
		}
	}

	private void cmdAdd_Click(object sender, EventArgs e)
	{
		UrlClass newSite;
		IList<UrlClass> cachedSites;

		newSite = new UrlClass();
		newSite.UserId = _userId;
		newSite.Url = "http://";

		_db.ORManager.SaveOrUpdate(newSite);

		//These will definitely be cached because we saved them when the list loaded
		cachedSites = SessionCache.GetSiteList(Session);
		cachedSites.Add(newSite);

		//Add the new list item and select it
		lstSites.Items.Add(new ListItem(newSite.Url, newSite.Id.ToString()));
		lstSites.SelectedIndex = lstSites.Items.Count - 1;

		displaySite(newSite);
	}

	private void cmdSave_Click(object sender, EventArgs e)
	{
		UrlClass selectedSite;

		selectedSite = getSelectedSite();
		if (selectedSite.Url != txtSiteUrl.Text)
		{
			selectedSite.Url = txtSiteUrl.Text;
			lstSites.SelectedItem.Text = selectedSite.Url;
		}

		//Note: we don't have to worry about the cache here, because
		//getSelectedSite() retrieved the actual object from the cache

		_db.ORManager.SaveOrUpdate(selectedSite);
	}

	private void lstSites_SelectedIndexChanged(object sender, EventArgs e)
	{
		displaySite(getSelectedSite());
	}

	private void populateSiteList()
	{
		ICriteria criteria;
		IList<UrlClass> pages;

		//Try to get the cached list first
		pages = SessionCache.GetSiteList(Session);

		if (pages == null)
		{
			criteria = _db.ORManager.Session.CreateCriteria(typeof (UrlClass)).Add(Expression.Eq("UserId", _userId));
			pages = criteria.List<UrlClass>();

			//Cache the latest page list
			SessionCache.SaveSiteList(Session, pages);
			_log.Debug("Saving the site list to the session cache");
		}
		else
		{
			_log.Debug("Loaded site list from the session cache");
		}

		lstSites.DataSource = pages;
		lstSites.DataTextField = "Url";
		lstSites.DataValueField = "Id";
		lstSites.DataBind();

		if (lstSites.Items.Count > 0)
			lstSites.SelectedIndex = 0;
	}

	private UrlClass getSelectedSite()
	{
		int siteId;
		IList<UrlClass> pages;
		UrlClass selectedSite = null;

		if (lstSites.SelectedIndex == -1)
			return null;

		siteId = int.Parse(lstSites.SelectedValue);

		pages = SessionCache.GetSiteList(Session);
		if (pages != null)
		{
			foreach (UrlClass currPage in pages)
			{
				if (currPage.Id == siteId)
				{
					selectedSite = currPage;
					break;
				}
			}
		}

		//If the site isn't cached, load the site
		if (selectedSite == null)
			selectedSite = _db.ORManager.Get<UrlClass>(siteId);

		return selectedSite;
	}

	private void displaySite(UrlClass site)
	{
		if (site == null)
		{
			txtSiteUrl.Enabled = false;
			txtSiteUrl.Text = "";
		}
		else
		{
			txtSiteUrl.Enabled = true;
			txtSiteUrl.Text = site.Url;
		}
	}
}