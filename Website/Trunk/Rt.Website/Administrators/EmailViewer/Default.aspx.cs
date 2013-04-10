using System;
using System.Web.UI;

public partial class Administrators_EmailViewer_Default : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected string GetMessage(string rawMessage, bool html)
	{
		if (!html)
			return Server.HtmlEncode(rawMessage);
		else
			return rawMessage;
	}
}