using System;
using System.Web.UI;
using Rt.Website;

public partial class Sign_Up_Sign_Up_Complete : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		cmdContinue.Click += cmdContinue_Click;
		AdministratorEmail.Text = GlobalSettings.AdministrativeEmail;
	}

	private void cmdContinue_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Members/");
	}
}