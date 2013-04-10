using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Rt.Framework.Applications.Correlation;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Web;
using Rt.Website;
using YTech.General.Drawing;
using YTech.General.Web;

public partial class Members_Reports_Correlation_Default : Page
{
	public const string SESSION_IMAGE_BYTES = "cdsib_{0}";
	private const string VIEWSTATE_SESSION_VARIABLE_NAME = "svn";

	/// <summary>
	///		Create and declare our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static int _sessionUniqueKey = 1;
	private Database _db;

	protected void Page_Load(object sender, EventArgs e)
	{
		_db = Global.GetDbConnection();

		if (!Page.IsPostBack)
			siteList.PopulatePageList();

		loadCorrelations();

		Plan plan = Global.LoadSubscriptionPlan(Membership.GetUser());

		Global.AddCommonJavaScript(Page);
	}

	private void loadCorrelations()
	{
		DataTable data;
		Dictionary<int, DiagramNode> nodes;
		List<DiagramNodeConnection> connections;
		int? selectedSiteId;

		nodes = new Dictionary<int, DiagramNode>();
		connections = new List<DiagramNodeConnection>();

		selectedSiteId = siteList.GetSelectedSiteId();
		if (selectedSiteId == null)
			return;

		data = _db.Report_DatasourceCorrelations(selectedSiteId.Value);
		//data = _db.Report_DatasourceCorrelations(8);

		for (int i = 0; i < data.Rows.Count; i++)
		{
			DiagramNode node;
			int nodeId, otherNodeId;
			string name, otherName;

			nodeId = (int) data.Rows[i]["ConfiguredDatasourceId"];
			name = (string) data.Rows[i]["Name"];
			if (nodes.ContainsKey(nodeId))
				node = nodes[nodeId];
			else
				nodes.Add(nodeId, new DiagramNode(DiagramGenerator.WrapText(name, 15), nodeId, null));

			otherNodeId = (int) data.Rows[i]["OtherDatasourceId"];
			otherName = (string) data.Rows[i]["OtherName"];
			if (nodes.ContainsKey(otherNodeId))
				node = nodes[otherNodeId];
			else
				nodes.Add(otherNodeId, new DiagramNode(DiagramGenerator.WrapText(otherName, 15), otherNodeId, null));

			string strength;

			strength = (string) data.Rows[i]["StrengthLabel"];

			if (strength != "None")
			{
				DiagramNodeConnection connection;
				connection = new DiagramNodeConnection();
				if (strength == "Large")
					connection.ConnectorWidth = 3;
				else if (strength == "Medium")
					connection.ConnectorWidth = 2;
				else
					connection.ConnectorWidth = 1;

				connection.FirstNode = nodes[nodeId];
				connection.SecondNode = nodes[otherNodeId];
				connections.Add(connection);
			}
		}

		processNodesAndConnections(connections);
	}

	private void processNodesAndConnections(List<DiagramNodeConnection> connections)
	{
		List<List<DiagramNodeConnection>> groups;
		List<byte[]> images;
		List<List<HotSpot>> hotspotLists;

		groups = DiagramGenerator.GetConnectionGroups(connections);
		images = new List<byte[]>();
		hotspotLists = new List<List<HotSpot>>();

		foreach (var currConnectionGroup in groups)
		{
			DiagramGenerator gen;
			byte[] bytes;
			Bitmap b;
			int width;

			//Set a width that will generally fit the nodes
			if (currConnectionGroup.Count >= 15)
				width = 750;
			else if (currConnectionGroup.Count <= 5)
				width = 300;
			else
				width = 500;

			gen = new DiagramGenerator(currConnectionGroup, width);
			b = gen.Generate();
			bytes = ImageUtilities.BitmapToBytes(b, ImageFormat.Png);
			images.Add(bytes);
			hotspotLists.Add(gen.Hotspots);
		}

		pnlNoCorrelations.Visible = images.Count == 0;

		if (images.Count > 0)
			displayDiagrams(images.ToArray(), hotspotLists);
	}

	private void displayDiagrams(byte[][] images, List<List<HotSpot>> hotspotLists)
	{
		string sessionVariableName;

		//Check if this page already had a session variable name
		sessionVariableName = ViewState[VIEWSTATE_SESSION_VARIABLE_NAME] as string;

		if (sessionVariableName == null)
		{
			_log.Debug("The viewstate doesn't contain a session variable name, so a new one will be generated");

			int sessionKey;

			sessionKey = Interlocked.Increment(ref _sessionUniqueKey);
			sessionVariableName = string.Format(SESSION_IMAGE_BYTES, sessionKey);
		}

		Session[sessionVariableName] = images;

		_log.DebugFormat("Stored image byte data in session variable '{0}'", sessionVariableName);

		//Remember the session so we can reuse it if they refresh of change the options
		ViewState[VIEWSTATE_SESSION_VARIABLE_NAME] = sessionVariableName;

		//Now add the image tags to the page
		for (int i = 0; i < images.Length; i++)
		{
			addImageControl(sessionVariableName, i, hotspotLists[i]);
		}
	}

	private void addImageControl(string sessionVariableName, int imageNumber, List<HotSpot> hotspots)
	{
		ImageMap webImage;
		UrlBuilder url;

		url = new UrlBuilder(ResolveUrl("~/Common/SessionImage.aspx"));
		url.Parameters.AddParameter(Common_SessionImage.PARAMETER_SESSION_VARIABLE_NAME, sessionVariableName);
		url.Parameters.AddParameter(Common_SessionImage.PARAMETER_IMAGE_NUMBER, imageNumber);

		webImage = new ImageMap();
		webImage.AlternateText = "Correlation Diagram";
		webImage.ImageUrl = url.ToString();
		foreach (HotSpot currHotspot in hotspots)
		{
			//The hotspot URL is incomplete.  The URL holds the ID list, so
			//we need to prepend the trend URL
			currHotspot.NavigateUrl = getTrendUrl(currHotspot.NavigateUrl);

			webImage.HotSpots.Add(currHotspot);
		}

		imagesPlaceholder.Controls.Add(webImage);
	}

	private string getTrendUrl(string datasourceList)
	{
		UrlBuilder url;

		url = new UrlBuilder(ResolveUrl("~/Members/Interactive-Report/"));
		url.Parameters.AddParameter(QueryParameters.QUERY_DATASOURCE_LIST, datasourceList);

		return url.ToString();
	}
}