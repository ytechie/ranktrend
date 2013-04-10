using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Services;
using Rt.Framework.Web;
using Rt.Website;

public partial class Sign_Up_Default : Page
{
	private const string COOKIE_PROMO_PARTICIPANT = "RTPP";
	private const string COOKIE_PROMO_PARTICIPANT_VALUE_HASH = "V2";
	private const string COOKIE_PROMO_PARTICIPANT_VALUE_ID = "V1";
	private const string SLTVAL = "their is not No TIme {0} LikE the preSentT (spellinG mistAKes IntenTIONal)";

	private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected void Page_Load(object sender, EventArgs e)
	{
		Database db = Global.GetDbConnection();

		if (!Page.IsPostBack)
		{
			// Wizard Step 1 - TOS
			LegalNoticeVersion termsOfService = db.GetLatestLegalNoticeVersion(GlobalSettings.SignUpTermsOfService);
			TermsOfService.Text = termsOfService.Notice;
		}
	}

	protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
	{
		Database db = Global.GetDbConnection();

		// Set user's approval to false so that it is "pending"
		MembershipUser membershipUser = Membership.GetUser(CreateUserWizard1.UserName);
		membershipUser.IsApproved = false;
		Membership.UpdateUser(membershipUser);

		// Save user's extended information to the database
		var userInformation = new UserInformation();
		userInformation.UserId = membershipUser.ProviderUserKey;
		userInformation.FirstName = ((TextBox) CreateUserWizardStep3.ContentTemplateContainer.FindControl("FirstName")).Text;
		userInformation.LastName = ((TextBox) CreateUserWizardStep3.ContentTemplateContainer.FindControl("LastName")).Text;
		userInformation.LeadKey = getLeadKey();
		db.ORManager.Save(userInformation);

		// Save user's agreement to the TOS
		LegalNoticeVersion termsOfService = db.GetLatestLegalNoticeVersion(GlobalSettings.SignUpTermsOfService);
		var tosAgreement = new LegalNoticeAgreement(termsOfService, membershipUser, true);
		db.SaveLegalNoticeAgreement(tosAgreement);

		// Update the user for the promotion participant if the user participated in a promotion
		int? signupPromoParticipantId = getSignupPromotionParticipantId();
		if (signupPromoParticipantId != null)
		{
			var promoParticipant = db.ORManager.Get<PromotionParticipant>(signupPromoParticipantId);

			// Make sure that the promotion participant isn't already assigned to another user
			if (promoParticipant.UserId == null)
			{
				promoParticipant.UserId = (Guid?) membershipUser.ProviderUserKey;
				db.ORManager.SaveOrUpdate(promoParticipant);
			}

			// Delete the promotion cookie
			HttpCookie cookie = Request.Cookies[COOKIE_PROMO_PARTICIPANT];
			if (cookie != null)
			{
				cookie.Expires = DateTime.Now.AddYears(-1);
				Response.SetCookie(cookie);
			}
		}

		// Save Verify User Email to email queue
		EmailTemplate emailTemplate = db.GetEmailTemplate(GlobalSettings.VerifyUserEmail);
		var emailMessage = new EmailMessage(emailTemplate);
		emailMessage.From = GlobalSettings.AdministrativeEmail;
		emailMessage.ApplyToUser(membershipUser, userInformation);
		db.SaveEmail(emailMessage);

		ThreadPool.QueueUserWorkItem(forceEmailEngineRun, membershipUser);

		Response.Redirect("Sign-Up-Complete.aspx");
	}

	private static void forceEmailEngineRun(object state)
	{
		MembershipUser membershipUser = null;
		try
		{
			membershipUser = (MembershipUser) state;
		}
		catch (Exception ex)
		{
			_log.Warn("Failed to cast state to a MembershipUser object.", ex);
		}

		try
		{
			RtEngines.EmailEngine.RunFullCycle();
		}
		catch (Exception ex)
		{
			string user = membershipUser == null ? string.Empty : membershipUser.ProviderUserKey + " ";
			_log.Warn(
				string.Format(
					"When user {0}signed up, an error occurred when attempting to force the email engine to run a cycle.", user), ex);
		}
	}

	private int? getSignupPromotionParticipantId()
	{
		// Check for valid promotion participant cookie
		HttpCookie promoParticipantCookie = Request.Cookies.Get(COOKIE_PROMO_PARTICIPANT);
		if (promoParticipantCookie != null && promoParticipantCookie.Values.Count == 2)
		{
			int id = int.Parse(promoParticipantCookie.Values[COOKIE_PROMO_PARTICIPANT_VALUE_ID]);
			string hash = promoParticipantCookie.Values[COOKIE_PROMO_PARTICIPANT_VALUE_HASH];
			if (hash == getPromoParticipantMd5Hash(id))
				return id;
		}

		return null;
	}

	private string getLeadKey()
	{
		HttpCookie leadKeyCookie = Request.Cookies.Get(LeadUrlRewriter.COOKIE_LEAD_KEY);
		if (leadKeyCookie != null)
			return leadKeyCookie.Value;
		return null;
	}

	private int? validatePromoCode(string promoCode)
	{
		Database db = Global.GetDbConnection();
		int? promoParticipantId = db.ValidatePromotion(promoCode);

		if (promoParticipantId != null)
		{
			var promoParticipantCookie = new HttpCookie(COOKIE_PROMO_PARTICIPANT);
			promoParticipantCookie.Values.Add(COOKIE_PROMO_PARTICIPANT_VALUE_ID, promoParticipantId.ToString());
			promoParticipantCookie.Values.Add(COOKIE_PROMO_PARTICIPANT_VALUE_HASH,
			                                  getPromoParticipantMd5Hash((int) promoParticipantId));
			Response.Cookies.Add(promoParticipantCookie);
		}

		return promoParticipantId;
	}

	// Hash an input string and return the hash as
	// a 32 character hexadecimal string.
	private static string getPromoParticipantMd5Hash(int id)
	{
		string input = string.Format(SLTVAL, id);

		// Create a new instance of the MD5CryptoServiceProvider object.
		MD5 md5Hasher = MD5.Create();

		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		var sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return sBuilder.ToString();
	}
}