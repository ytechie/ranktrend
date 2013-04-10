using System;
using System.Web.UI;
using YTech.General.Web;

public partial class a_Default : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		RedirectUtilities.Redirect301(Response, ResolveUrl("~/"));
	}
}