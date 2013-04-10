using System;
using System.Web.UI;
using Bewise.Web.UI.WebControls;

public partial class Members_Advanced_Charting_Default : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			siteList.PopulatePageList();
			setSiteIdFlashVar();
		}

		siteList.SelectedItemChanged += siteList_changed;
	}

	private void siteList_changed(object s, EventArgs e)
	{
		setSiteIdFlashVar();
	}

	private void setSiteIdFlashVar()
	{
		int? selectedSiteId = siteList.GetSelectedSiteId();

		flash.FlashVarsCollection.Add(new FlashVarItem("siteId", selectedSiteId.ToString()));
	}
}