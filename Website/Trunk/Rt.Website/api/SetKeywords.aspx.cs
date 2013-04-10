using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class api_UpdateKeywords : Page
{
	private List<string> _keywords;
	private int _siteId;

	protected void Page_Load(object sender, EventArgs e)
	{
		getParameters();
	}

	private void getParameters()
	{
		if (!int.TryParse(Request.Form["siteId"], out _siteId))
			throw new Exception("Invalid parameter value for siteId");

		string keywords = Request.Form["keywords"];
		_keywords = new List<string>(keywords.Split(",".ToCharArray()));
	}
}