using System;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Website;

public partial class Members_Tray_Application_Default : Page
{
	private const string PARAM_OS = "OS";

	private SupportedOSs ClientOS
	{
		get
		{
			string osString;
			osString = Request.QueryString[PARAM_OS];
			if (string.IsNullOrEmpty(osString))
				return getOS();
			else if (Regex.IsMatch(osString, SupportedOSs.WinXP.ToString("g"), RegexOptions.IgnoreCase))
				return SupportedOSs.WinXP;
			else if (Regex.IsMatch(osString, SupportedOSs.MacOSX.ToString("g"), RegexOptions.IgnoreCase))
				return SupportedOSs.MacOSX;
			else
				return SupportedOSs.WinXP;
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			int? legalNoticeId = GlobalSettings.TrayApplicationLicenseAgreement;

			Agree.Enabled = (legalNoticeId != null);
			Disagree.Enabled = (legalNoticeId != null);

			if (legalNoticeId == null)
			{
				setForNoLegalNotice();
			}
			else
			{
				Database db = Global.GetDbConnection();
				LegalNoticeVersion legalNotice = db.GetLatestLegalNoticeVersion((int) legalNoticeId);

				if (legalNotice == null)
				{
					setForNoLegalNotice();
				}
				else
				{
					bool agreed = db.HasAgreedToNotice((int) legalNotice.Id, Membership.GetUser().ProviderUserKey);
					LicenseAgreementPanel.Visible = !agreed;
					DownloadApplicationPanel.Visible = agreed;
					if (!agreed)
						LicenseAgreement.Text = legalNotice.Notice;
					else
						initClientOS();
				}
			}
		}
	}

	protected void initClientOS()
	{
		WindowsRequirements.Visible = false;
		MacOSRequirements.Visible = false;

		SupportedOSs clientOS = ClientOS;
		switch (clientOS)
		{
			case SupportedOSs.WinXP:
				string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
				if (userAgent != null)
				{
					if (userAgent.IndexOf(".NET CLR 3.0.04506.30") >= 0)
						NetVersionMessage.Text = "<br /><i>* You appear to already have .NET 3.0 installed. *</i>";
					else if (userAgent.IndexOf(".NET CLR 2.0.50727") >= 0)
						NetVersionMessage.Text = "<br /><i>* You appear to already have .NET 2.0 installed. *</i>";
				}

				WindowsRequirements.Visible = true;

				DownloadApplication.AlternateText = "Download for Windows";
				DownloadApplication.ImageUrl = "~/Images/Download-For-Windows-Button.gif";

				break;
			case SupportedOSs.MacOSX:

				MacOSRequirements.Visible = true;

				DownloadApplication.AlternateText = "Download for Mac OS X";
				DownloadApplication.ImageUrl = "~/Images/Download-For-MacOSX-Button.gif";

				break;
			default:
				throw new NotSupportedException(string.Format("{0} is not a supported client operating system.",
				                                              clientOS.ToString("g")));
		}
	}

	protected void Agree_Click(object sender, EventArgs e)
	{
		setAgreement(true);
		LicenseAgreementPanel.Visible = false;
		DownloadApplicationPanel.Visible = true;
	}

	protected void Disagree_Click(object sender, EventArgs e)
	{
		setAgreement(false);
		Response.Redirect("~/Members/");
	}

	protected void DownloadApplication_Click(object sender, EventArgs e)
	{
		SupportedOSs clientOS = ClientOS;
		switch (clientOS)
		{
			case SupportedOSs.WinXP:
				Response.Redirect("~/Members/Tray-Application/RankTrend-Tray-Application.msi");
				break;
			case SupportedOSs.MacOSX:
				Response.Redirect("~/Members/Tray-Application/RankTrend-Tray-Application.dmg");
				break;
			default:
				throw new NotSupportedException(
					string.Format("{0} is not a supported client operating system.  There is no download for this operating system.",
					              clientOS.ToString("g")));
		}
	}

	private void setForNoLegalNotice()
	{
		Agree.Enabled = false;
		Disagree.Enabled = false;

		LicenseAgreementPanel.Visible = true;
		LicenseAgreement.Text = "Insert License Agreement here.";

		DownloadApplicationPanel.Visible = false;
	}

	private void setAgreement(bool agree)
	{
		Database db = Global.GetDbConnection();
		LegalNoticeVersion legalNotice = db.GetLatestLegalNoticeVersion((int) GlobalSettings.TrayApplicationLicenseAgreement);
		var agreement = new LegalNoticeAgreement(legalNotice, Membership.GetUser(), agree);
		db.SaveLegalNoticeAgreement(agreement);
	}

	private SupportedOSs getOS()
	{
		string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
		if (userAgent != null)
		{
			if (userAgent.IndexOf("Windows NT 5.1") >= 0
			    || userAgent.IndexOf("Windows NT 5.2") >= 0
			    || userAgent.IndexOf("Windows NT 6.0") >= 0)
				return SupportedOSs.WinXP;
			else if (userAgent.IndexOf("Mac OS X") >= 0)
				return SupportedOSs.MacOSX;
			else
				return SupportedOSs.WinXP;
		}
		else
			return SupportedOSs.WinXP;
	}

	#region Nested type: SupportedOSs

	private enum SupportedOSs
	{
		WinXP,
		MacOSX
	}

	#endregion
}