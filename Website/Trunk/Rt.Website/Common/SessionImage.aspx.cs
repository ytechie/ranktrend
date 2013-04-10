using System;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.UI;
using log4net;
using YTech.General.Drawing;
using YTech.General.Web;

public partial class Common_SessionImage : Page
{
	public const string PARAMETER_IMAGE_NUMBER = "in";
	public const string PARAMETER_SESSION_VARIABLE_NAME = "svn";

	public const string SESSION_IMAGE_BYTES = "cdsib_{0}";
	private const string VIEWSTATE_SESSION_VARIABLE_NAME = "svn";

	/// <summary>
	///		Create and declare our logger.
	/// </summary>
	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static int _sessionUniqueKey = 1;
	private byte[] _imageBytes;
	private int _imageNumber;
	private string _sessionVariableName;

	protected void Page_Load(object sender, EventArgs e)
	{
		getParameters();
		loadImage();
		renderImage();
	}

	private void getParameters()
	{
		string parameter;

		parameter = Request.QueryString[PARAMETER_IMAGE_NUMBER];
		if (string.IsNullOrEmpty(parameter))
			throw new ArgumentException("Missing image number parameter");
		else
			_imageNumber = int.Parse(parameter);

		_sessionVariableName = Request.QueryString[PARAMETER_SESSION_VARIABLE_NAME];
		if (string.IsNullOrEmpty(_sessionVariableName))
			throw new ArgumentException("Missing session variable name parameter");
	}

	private void loadImage()
	{
		byte[][] images;

		images = Session[_sessionVariableName] as byte[][];
		if (images == null)
			throw new Exception("Could not find images in the session");

		_imageBytes = images[_imageNumber];
	}

	private void renderImage()
	{
		ImageUtilities.WritePngImage(Response, _imageBytes);
	}

	public static string StoreImageForDisplay(byte[] bytes, StateBag pageViewstate)
	{
		UrlBuilder url;

		string sessionVariableName;

		//Check if this page already had a session variable name
		sessionVariableName = pageViewstate[VIEWSTATE_SESSION_VARIABLE_NAME] as string;

		if (sessionVariableName == null)
		{
			_log.Debug("The viewstate doesn't contain a session variable name, so a new one will be generated");

			int sessionKey;

			sessionKey = Interlocked.Increment(ref _sessionUniqueKey);
			sessionVariableName = string.Format(SESSION_IMAGE_BYTES, sessionKey);
		}

		HttpContext.Current.Session[sessionVariableName] = new[] {bytes};

		_log.DebugFormat("Stored image byte data in session variable '{0}'", sessionVariableName);

		//Remember the session so we can reuse it if they refresh of change the options
		pageViewstate[VIEWSTATE_SESSION_VARIABLE_NAME] = sessionVariableName;

		url = new UrlBuilder(((Page) HttpContext.Current.Handler).ResolveUrl("~/Common/SessionImage.aspx"));
		url.Parameters.AddParameter(PARAMETER_SESSION_VARIABLE_NAME, sessionVariableName);
		url.Parameters.AddParameter(PARAMETER_IMAGE_NUMBER, 0);

		return url.ToString();
	}
}