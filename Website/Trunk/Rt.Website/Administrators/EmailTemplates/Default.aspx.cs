using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rt.Framework.Db.SqlServer;
using Rt.Website;
using YTech.General.Web;

public partial class Administrators_EmailTemplates_Default : Page
{
	public static string GetLoadUrl()
	{
		var page = new Administrators_EmailTemplates_Default();
		var url = new UrlBuilder("~/Administrators/EmailTemplates/");
		return url.ToString();
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			initEmailTemplatesDataGrid();
		}
	}

	protected void Delete_Click(object sender, EventArgs e)
	{
		var Delete = (LinkButton) sender;
		Database db = Global.GetDbConnection();

		db.DeleteEmailTemplate(int.Parse(Delete.CommandArgument));
		initEmailTemplatesDataGrid();
	}

	protected string limit(string text, int size)
	{
		return text.Substring(0, text.Length > size ? size : text.Length);
	}

	protected string htmlEncode(string text)
	{
		return Server.HtmlEncode(text);
	}

	private void initEmailTemplatesDataGrid()
	{
		Database db = Global.GetDbConnection();

		dgEmailTemplates.DataSource = db.GetEmailTemplatesTable();
		dgEmailTemplates.DataBind();
	}
}