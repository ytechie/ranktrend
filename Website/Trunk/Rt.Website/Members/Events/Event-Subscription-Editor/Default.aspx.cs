using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using MichaelBrumm.Win32;
using NHibernate;
using NHibernate.Expression;
using Rss;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Website;

public partial class Members_Events_Event_Subscription_Editor_Default : Page
{
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		siteList.SelectedItemChanged += siteList_SelectedItemChanged;
		lstSubscriptions.SelectedIndexChanged += lstSubscriptions_SelectedIndexChanged;
		cmdAdd.Click += cmdAdd_Click;
		cmdDelete.Click += cmdDelete_Click;
		cmdSave.Click += cmdSave_Click;
		cmdReadFeedTitle.Click += cmdReadFeedTitle_Click;

		if (!Page.IsPostBack)
		{
			siteList.PopulatePageList();
			categoryList.PopulateCategoryList(siteList);
			populateSubscriptionList();
			displaySubscription(getSelectedSubscription());
		}
	}

	private void cmdReadFeedTitle_Click(object sender, EventArgs e)
	{
		RssFeed feed;

		if (txtRssUrl.Text == "" || txtRssUrl.Text == "http://")
			return;

		try
		{
			feed = RssFeed.Read(txtRssUrl.Text);
		}
		catch (Exception)
		{
			txtName.Text = "Unknown (Failure Reading Title)";
			return;
		}

		if (feed.Channels.Count == 0 || feed.Exceptions.Count > 0)
		{
			txtName.Text = "Unknown (Failure Reading Title)";
			return;
		}

		//Success!
		txtName.Text = feed.Channels[0].Title;

		if (txtDescription.Text.Trim() == "")
			txtDescription.Text = feed.Channels[0].Description;
	}

	private void cmdAdd_Click(object sender, EventArgs e)
	{
		EventRssSubscription subscription;

		subscription = new EventRssSubscription();
		subscription.Name = "Untitled RSS Feed";
		subscription.Url = siteList.GetSelectedSite();
		subscription.RssUrl = "http://";
		subscription.ErrorCount = 0;

		_db.ORManager.SaveOrUpdate(subscription);

		//Add the new item to the list
		lstSubscriptions.Items.Add(new ListItem(subscription.Name, subscription.Id.ToString()));
		lstSubscriptions.SelectedIndex = lstSubscriptions.Items.Count - 1;

		displaySubscription(subscription);
	}

	private void cmdDelete_Click(object sender, EventArgs e)
	{
		EventRssSubscription subscription;

		subscription = getSelectedSubscription();
		ThreadPool.QueueUserWorkItem(delegate { _db.ORManager.Delete(subscription); });
		lstSubscriptions.Items.Remove(lstSubscriptions.SelectedItem);

		if (lstSubscriptions.Items.Count == 0)
		{
			displaySubscription(null);
		}
		else
		{
			lstSubscriptions.SelectedIndex = lstSubscriptions.Items.Count - 1;
			displaySubscription(getSelectedSubscription());
		}
	}

	private void cmdSave_Click(object sender, EventArgs e)
	{
		EventRssSubscription subscription;

		subscription = getSelectedSubscription();
		if (subscription == null)
			return;

		if (subscription.Name != txtName.Text)
		{
			subscription.Name = txtName.Text;
			//Update the name in the list
			lstSubscriptions.SelectedItem.Text = txtName.Text;
		}

		subscription.Description = txtDescription.Text;

		subscription.EventCategory = categoryList.GetSelectedCategory();
		subscription.ErrorCount = 0;

		if (subscription.RssUrl != txtRssUrl.Text)
		{
			subscription.RssUrl = txtRssUrl.Text;

			//Update the feed right away so that events are immediately available
			ThreadPool.QueueUserWorkItem(delegate { RtEngines.RssEventEngine.ProcessRssSubscription(subscription); });
		}

		ThreadPool.QueueUserWorkItem(delegate { _db.ORManager.SaveOrUpdate(subscription); });
	}

	private void lstSubscriptions_SelectedIndexChanged(object sender, EventArgs e)
	{
		displaySubscription(getSelectedSubscription());
	}

	private void siteList_SelectedItemChanged(object sender, EventArgs e)
	{
		categoryList.PopulateCategoryList(siteList);
		populateSubscriptionList();
		displaySubscription(getSelectedSubscription());
	}

	private void populateSubscriptionList()
	{
		IList<EventRssSubscription> subscriptions;
		ICriteria criteria;
		int? urlId;

		urlId = siteList.GetSelectedSiteId();
		if (urlId == null)
			return;

		criteria = _db.ORManager.Session.CreateCriteria(typeof (EventRssSubscription)).Add(Expression.Eq("Url.Id", urlId));
		subscriptions = criteria.List<EventRssSubscription>();

		lstSubscriptions.DataSource = subscriptions;
		lstSubscriptions.DataTextField = "Name";
		lstSubscriptions.DataValueField = "Id";
		lstSubscriptions.DataBind();

		//Select the first item if possible
		if (lstSubscriptions.Items.Count > 0)
			lstSubscriptions.SelectedIndex = 0;
	}

	private EventRssSubscription getSelectedSubscription()
	{
		int subscriptionId;

		if (lstSubscriptions.SelectedIndex == -1)
			return null;

		subscriptionId = int.Parse(lstSubscriptions.SelectedValue);

		return _db.ORManager.Get<EventRssSubscription>(subscriptionId);
	}

	private void displaySubscription(EventRssSubscription ers)
	{
		Win32TimeZone timeZone;

		timeZone = TimeZones.GetTimeZone(Profile.TimeZoneIndex);

		if (ers == null)
		{
			txtName.Enabled = false;
			txtName.Text = "";
			cmdReadFeedTitle.Enabled = false;
			txtDescription.Enabled = false;
			txtDescription.Text = "";
			txtRssUrl.Enabled = false;
			txtRssUrl.Text = "";
			categoryList.Enabled = false;
			categoryList.SelectAnyItem();
			lblLastCheck.Text = "";
		}
		else
		{
			txtName.Enabled = true;
			txtName.Text = ers.Name;
			cmdReadFeedTitle.Enabled = true;
			txtDescription.Enabled = true;
			txtDescription.Text = ers.Description;
			txtRssUrl.Enabled = true;
			txtRssUrl.Text = ers.RssUrl;
			categoryList.Enabled = true;
			categoryList.SetSelectedCategory(ers.EventCategory);
			if (ers.LastCheck == null)
				lblLastCheck.Text = "Never";
			else
				lblLastCheck.Text = timeZone.ToLocalTime((DateTime) ers.LastCheck).ToString();
		}
	}
}