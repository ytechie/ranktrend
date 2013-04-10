using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_CommonControls_ConfiguredDatasourceList : UserControl
{
	private Database _db;

	private Database DB
	{
		get
		{
			if (_db == null)
				_db = Global.GetDbConnection();

			return _db;
		}
	}

	public bool AutoPostBack
	{
		get { return ddlDatasources.AutoPostBack; }
		set { ddlDatasources.AutoPostBack = value; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		ddlDatasources.SelectedIndexChanged += ddlDatasources_SelectedIndexChanged;
	}

	private void ddlDatasources_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (pnlSubTypes.Visible)
			populateSubTypeList();
	}

	public void ShowSubTypeList()
	{
		pnlSubTypes.Visible = true;
		populateSubTypeList();
	}

	public void PopulateDatasourceList(UrlClass site)
	{
		DataTable dt;

		if (site == null)
		{
			ddlDatasources.Items.Clear();
			return;
		}

		dt = DB.Ds_GetPageDatasourceList(site.Id);

		ddlDatasources.DataSource = dt;
		ddlDatasources.DataTextField = "DisplayName";
		ddlDatasources.DataValueField = "Id";
		ddlDatasources.DataBind();
	}

	private void populateSubTypeList()
	{
		IList<DatasourceSubType> subTypes;
		ICriteria criteria;
		ConfiguredDatasource selectedDatasourceType;

		selectedDatasourceType = GetSelectedItem();

		if (selectedDatasourceType == null)
		{
			subTypes = new List<DatasourceSubType>();
		}
		else
		{
			criteria =
				DB.ORManager.Session.CreateCriteria(typeof (DatasourceSubType)).Add(
					Expression.Eq("DatasourceType.Id", selectedDatasourceType.DatasourceType.Id));
			subTypes = criteria.List<DatasourceSubType>();
		}

		if (subTypes.Count == 0)
		{
			ddlSubTypes.Enabled = false;
			ddlSubTypes.Items.Clear();
			ddlSubTypes.Items.Add(new ListItem("N/A", ""));
			ddlSubTypes.SelectedIndex = 0;
		}
		else
		{
			ddlSubTypes.Enabled = true;

			ddlSubTypes.DataSource = subTypes;
			ddlSubTypes.DataTextField = "Name";
			ddlSubTypes.DataValueField = "Id";
			ddlSubTypes.DataBind();
		}
	}

	public ConfiguredDatasource GetSelectedItem()
	{
		int? selectedId;

		selectedId = GetSelectedDatasourceId();
		if (selectedId == null)
			return null;

		return DB.ORManager.Get<ConfiguredDatasource>((int) selectedId);
	}

	public int? GetSelectedDatasourceId()
	{
		int selectedId;

		if (ddlDatasources.SelectedIndex == -1)
			return null;

		selectedId = int.Parse(ddlDatasources.SelectedValue);

		return selectedId;
	}

	public DatasourceSubType GetSelectedSubType()
	{
		int? selectedId;

		selectedId = GetSelectedSubTypeId();

		if (selectedId == null)
			return null;

		return DB.ORManager.Get<DatasourceSubType>((int) selectedId);
	}

	public int? GetSelectedSubTypeId()
	{
		int selectedId;

		if (ddlSubTypes.SelectedIndex == -1 || ddlSubTypes.SelectedValue.Length == 0)
			return null;

		selectedId = int.Parse(ddlSubTypes.SelectedValue);

		return selectedId;
	}
}