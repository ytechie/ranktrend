using System;
using System.Web.UI;
using YTech.General.Web;

public partial class Administrators_LegalNotices_Default : Page
{
	public static string GetLoadUrl()
	{
		var page = new Administrators_LegalNotices_Default();
		var url = new UrlBuilder("~/Administrators/LegalNotices/");
		return url.ToString();
	}

	protected void Page_Load(object sender, EventArgs e)
	{
	}
}