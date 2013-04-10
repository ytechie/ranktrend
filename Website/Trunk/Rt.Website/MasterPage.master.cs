using System;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rt.Website;
using YTech.General.Web.JSProxy;

public partial class MasterPage : System.Web.UI.MasterPage
{
	public JSProxyGenerator ControlProxies
	{
		get { return controlProxies; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		Global.AddCommonJavaScript(Page);

		addMenuItems();

		if (Request.Url.ToString().Contains("/Members/"))
		{
			MembersCss.Visible = true;
			JQueryCalendarCss.Visible = true;
		}
		if (Request.Url.ToString().Contains("/Administrators/"))
		{
			AdministratorsCss.Visible = true;
			MembersCss.Visible = true;
		}

		LoginStatus1.LoggedOut += LoginStatus1_LoggedOut;
	}

	private void LoginStatus1_LoggedOut(object sender, EventArgs e)
	{
		SessionCache.SaveSiteList(Session, null);
		Session[SessionCache.USER_PLAN] = null;
	}

	private void addMenuItems()
	{
		SiteMapNode currMenuNode;

		addMainMenuItem(SiteMap.RootNode);

		for (int i = 0; i < SiteMap.RootNode.ChildNodes.Count; i++)
		{
			currMenuNode = SiteMap.RootNode.ChildNodes[i];
			addMainMenuItem(currMenuNode);
		}
	}

	private void addMainMenuItem(SiteMapNode itemNode)
	{
		HtmlGenericControl currMenuItem;
		HyperLink link;

		link = new HyperLink();
		link.Text = itemNode.Title;
		link.ToolTip = itemNode.Description;

		if (itemNode == SiteMap.CurrentNode)
			link.CssClass = "active";
		else
			link.NavigateUrl = itemNode.Url;

		currMenuItem = new HtmlGenericControl("li");
		currMenuItem.Controls.Add(link);


		lstMenu.Controls.Add(currMenuItem);
	}
}