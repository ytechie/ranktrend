using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataStreams.Csv;
using log4net;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services.DataEngine.DataSources;
using Rt.Website;

public partial class Members_Import_Data_Wizard_Default : Page
{
	private const string SESSION_DATA_NAME = "ImportWizard_ImportData";

	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	private Database _db;
	private Guid _userId;

	protected void Page_Load(object sender, EventArgs e)
	{
		_userId = (Guid) Membership.GetUser().ProviderUserKey;
		_db = Global.GetDbConnection();

		wizard.ActiveStepChanged += wizard_ActiveStepChanged;
		wizard.NextButtonClick += wizard_NextButtonClick;
		wizard.FinishButtonClick += wizard_FinishButtonClick;

		if (!Page.IsPostBack)
		{
			//pouplateDatasourceList();
		}

		selectDetailPanel(false);
	}

	private void wizard_ActiveStepChanged(object sender, EventArgs e)
	{
		if (wizard.ActiveStepIndex == 3)
		{
			pouplateDatasourceList();
		}
	}

	private ImportType getSelectedImportTypeId()
	{
		if (ddlFileType.SelectedValue == "1")
			return ImportType.Adsense;
		else if (ddlFileType.SelectedValue == "2")
			return ImportType.GenericCsv;
		else if (ddlFileType.SelectedValue == "3")
			return ImportType.GenericTsv;
		else if (ddlFileType.SelectedValue == "4")
			return ImportType.DigitalPointBacklinks;
		else
			throw new NotImplementedException();
	}

	private void wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
	{
		saveDataToDB();
	}

	private void wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
	{
		if (e.NextStepIndex == 2)
		{
			if (!csvUpload.HasFile)
			{
				e.Cancel = true;
				return;
			}
			else
			{
				processUploadFile();
			}
		}
	}

	//Makes one of the detail panels visible depending on which
	//type the user has selected
	private void selectDetailPanel(bool showError)
	{
		ImportType fileTypeId;

		pnlFilterAdsense.Visible = false;
		pnlTypeGeneric.Visible = false;
		pnlTypeError.Visible = false;

		if (showError)
		{
			pnlTypeError.Visible = true;
		}
		else
		{
			fileTypeId = getSelectedImportTypeId();

			if (fileTypeId == ImportType.Adsense)
				pnlFilterAdsense.Visible = true;
			else if (fileTypeId == ImportType.DigitalPointBacklinks)
				pnlTypeDigitalPointBacklinks.Visible = true;
			else if (fileTypeId == ImportType.GenericCsv || fileTypeId == ImportType.GenericTsv)
				pnlTypeGeneric.Visible = true;
		}
	}

	private void pouplateDatasourceList()
	{
		IList<ConfiguredDatasource> cds;
		ICriteria queryCriteria;
		ImportType fileTypeId;

		//Don't populate the list twice
		if (ddlDatasources.Items.Count > 0)
			return;

		fileTypeId = getSelectedImportTypeId();

		queryCriteria = _db.ORManager.Session.CreateCriteria(typeof (ConfiguredDatasource));

		//Try to narrow the datasource list
		if (fileTypeId == ImportType.Adsense)
			queryCriteria.Add(Expression.Eq("DatasourceType.Id", 3));
		else if (fileTypeId == ImportType.DigitalPointBacklinks)
			queryCriteria.Add(Expression.Eq("DatasourceType.Id", 6));
		else
			queryCriteria.Add(Expression.Not(Expression.Eq("DatasourceType.Id", 3)));

		//Make sure we only get the datasources for the logged in user
		queryCriteria.CreateCriteria("Url").Add(Expression.Eq("UserId", _userId));
		queryCriteria.AddOrder(Order.Asc("Url.Id")).AddOrder(Order.Asc("DatasourceType.Id"));
		cds = queryCriteria.List<ConfiguredDatasource>();

		foreach (ConfiguredDatasource cd in cds)
		{
			ddlDatasources.Items.Add(new ListItem(cd.DisplayNameWithUrl, cd.Id.ToString()));
		}
	}

	private void processUploadFile()
	{
		CsvReader reader;
		DataTable uploadedData;
		TextReader tr;
		ImportType fileTypeId;
		char delimeter;

		fileTypeId = getSelectedImportTypeId();

		//This will automatically detect the byte order marks
		tr = new StreamReader(csvUpload.PostedFile.InputStream, true);

		switch (fileTypeId)
		{
			case ImportType.GenericTsv:
			case ImportType.Adsense:
				delimeter = '\t';
				break;
			default:
				delimeter = ',';
				break;
		}

		//Get the file they chose to upload
		reader = new CsvReader(tr, delimeter);

		uploadedData = reader.ReadToEnd();

		if (uploadedData.Columns.Count < 2)
		{
			lblError.Text =
				"Your data file appears to only have 1 column.  It must contain at least a date column and a value column";
			return;
		}

		//Save the data to the session for later use
		Session[SESSION_DATA_NAME] = uploadedData;

		switch (fileTypeId)
		{
			case ImportType.Adsense:
				getAdsenseParameters(uploadedData);
				break;
			case ImportType.DigitalPointBacklinks:
				getDigitalPointBacklinksParameters(uploadedData);
				break;
			case ImportType.GenericCsv:
			case ImportType.GenericTsv:
				getGenericParameters(uploadedData);
				break;
		}
	}

	private ConfiguredDatasource getSelectedDatasource()
	{
		int datasourceId;
		ConfiguredDatasource cd;

		datasourceId = int.Parse(ddlDatasources.SelectedValue);
		cd = _db.ORManager.Get<ConfiguredDatasource>(datasourceId);

		return cd;
	}

	private void saveDataToDB()
	{
		DataTable data;
		int datasourceId;
		ImportType fileTypeId;

		//Load the data from the session
		data = Session[SESSION_DATA_NAME] as DataTable;
		if (data == null)
		{
			_log.Info(
				"NULL session data located, which probably means there was an error message, and the user chose to ignore it.");
			return;
		}

		datasourceId = int.Parse(ddlDatasources.SelectedValue);

		fileTypeId = getSelectedImportTypeId();
		switch (fileTypeId)
		{
			case ImportType.Adsense:
				processAdsenseData(data, datasourceId);
				break;
			case ImportType.DigitalPointBacklinks:
				processDigitalPointBacklinks(data, datasourceId);
				break;
			case ImportType.GenericTsv:
			case ImportType.GenericCsv:
				processGenericData(data, datasourceId);
				break;
			default:
				throw new Exception();
		}
	}

	#region Specific data processing methods

	private void processAdsenseData(DataTable data, int configuredDatasourceId)
	{
		RawDataValue[] dataValues;

		dataValues = GoogleAdsense.ProcessAdsenseData(data, ddlChannelNames.SelectedItem.Text);

		//We need to set the configured datasource ID on all of the values
		foreach (RawDataValue currValue in dataValues)
		{
			currValue.ConfiguredDatasourceId = configuredDatasourceId;
		}

		_db.BulkInsertRawData(dataValues);
	}

	private void processDigitalPointBacklinks(DataTable data, int configuredDatasourceId)
	{
		List<RawDataValue> dataValues;

		//Remove rows that are not for the selected site
		for (int i = data.Rows.Count - 1; i > 0; i--)
		{
			if (!data.Rows[i]["URL"].Equals(ddlDPBacklinksSite.SelectedItem.Text))
				data.Rows.Remove(data.Rows[i]);
		}

		dataValues = new List<RawDataValue>();

		dataValues.AddRange(RawDataValue.GetRawDataForBulkInsert(data, "Date", "Back Links", configuredDatasourceId, null));

		_db.BulkInsertRawData(dataValues.ToArray());
	}

	private void processGenericData(DataTable data, int configuredDatasourceId)
	{
		RawDataValue[] dataValues;

		dataValues = RawDataValue.GetRawDataForBulkInsert(data, ddlTimestampColumn.SelectedItem.Text,
		                                                  ddlValueColumn.SelectedItem.Text, configuredDatasourceId, null);

		_db.BulkInsertRawData(dataValues);
	}

	#endregion

	#region Parameter getters for the different import types

	private void getAdsenseParameters(DataTable data)
	{
		List<string> channelNames;
		string currChannelName;

		//Remove the last row, because it contains the totals
		data.Rows.Remove(data.Rows[data.Rows.Count - 1]);

		channelNames = new List<string>();

		if (data.Columns.Contains("Channel"))
		{
			//Build a list of channel names

			foreach (DataRow row in data.Rows)
			{
				currChannelName = (string) row["Channel"];
				if (!channelNames.Contains(currChannelName))
					channelNames.Add(currChannelName);
			}
			ddlChannelNames.Enabled = true;
		}
		else
		{
			channelNames.Add("<Default>");
			ddlChannelNames.Enabled = false;
		}

		//Populate the channel names
		ddlChannelNames.DataSource = channelNames;
		ddlChannelNames.DataBind();
	}

	private void getDigitalPointBacklinksParameters(DataTable data)
	{
		List<string> siteNames;
		string currSiteName;

		siteNames = new List<string>();

		//Loop through the data, ignoring the header
		foreach (DataRow row in data.Rows)
		{
			currSiteName = (string) row["URL"];
			if (!siteNames.Contains(currSiteName) && currSiteName.Length > 0)
				siteNames.Add(currSiteName);
		}

		ddlDPBacklinksSite.DataSource = siteNames;
		ddlDPBacklinksSite.DataBind();
	}

	private void getGenericParameters(DataTable data)
	{
		foreach (DataColumn currColumn in data.Columns)
		{
			ddlTimestampColumn.Items.Add(currColumn.ColumnName);
			ddlValueColumn.Items.Add(currColumn.ColumnName);
		}

		//Try to autoselect some columns that look correct
		if (data.Columns.Contains("Timestamp"))
			ddlTimestampColumn.Items.FindByText("Timestamp").Selected = true;
		else if (data.Columns.Contains("Date"))
			ddlTimestampColumn.Items.FindByText("Date").Selected = true;

		if (data.Columns.Contains("Value"))
			ddlValueColumn.Items.FindByText("Value").Selected = true;
		else
			ddlValueColumn.SelectedIndex = 1;
	}

	#endregion

	#region Nested type: ImportType

	private enum ImportType
	{
		Adsense = 1,
		GenericCsv = 2,
		GenericTsv = 3,
		DigitalPointBacklinks = 4
	}

	#endregion
}