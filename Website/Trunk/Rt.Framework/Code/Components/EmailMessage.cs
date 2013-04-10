using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Security;
using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	public class EmailMessage
	{
		public const string TOKEN_EMAIL = "{USERS_EMAIL}";
		public const string TOKEN_FIRSTNAME = "{USERS_FIRST_NAME}";
		public const string TOKEN_FULLNAME = "{USERS_FULL_NAME}";
		public const string TOKEN_HOMEPAGE_LINK = "{HOMEPAGE_LINK}";
		public const string TOKEN_HOMEPAGE_URL = "{HOMEPAGE_URL}";
		public const string TOKEN_LASTNAME = "{USERS_LAST_NAME}";
		public const string TOKEN_LOGO = "{LOGO}";
		public const string TOKEN_USERNAME = "{USERS_USERNAME}";
		public const string TOKEN_USERUID = "{USERS_UID}";

		private string _from;
		private bool _html;
		private int? _id;
		private DateTime _lastTry;
		private string _message;
		private int _numberOfTries;
		private DateTime _queuedOn;
		private DateTime _sentOn;
		private string _subject;
		private string _toAddress;
		private string _toName;
		private object _userId;

		public EmailMessage()
		{
		}

		public EmailMessage(EmailTemplate template)
		{
			Subject = template.Subject;
			Message = template.Message;
			Html = template.Html;

			ReplaceGeneralTokens();
		}

		#region Public Properties

		/// <summary>
		///		The unique identifier for this object in
		///		the database.  If this object was not loaded
		///		from the database, an exception will be thrown.
		/// </summary>
		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		The "From" field to use when sending the email.
		/// </summary>
		[FieldMapping("From")]
		public string From
		{
			get { return _from; }
			set { _from = value; }
		}

		/// <summary>
		///		The name of the person receiving the email.  This
		///		can be left NULL if it is not known.
		/// </summary>
		[FieldMapping("ToName")]
		public string ToName
		{
			get { return _toName; }
			set { _toName = value; }
		}

		/// <summary>
		///		The address to send the email to.
		/// </summary>
		[FieldMapping("ToAddress")]
		public string ToAddress
		{
			get { return _toAddress; }
			set { _toAddress = value; }
		}

		/// <summary>
		///		The subject of the email.
		/// </summary>
		[FieldMapping("Subject")]
		public string Subject
		{
			get { return _subject; }
			set { _subject = value; }
		}

		/// <summary>
		///		If true, the email will be treated as an HTML email.  Only
		///		set this to true if you are using HTML.  Text is generally
		///		accepted better.
		/// </summary>
		[FieldMapping("Html")]
		public bool Html
		{
			get { return _html; }
			set { _html = value; }
		}

		/// <summary>
		///		The <see cref="DateTime"/> that the email was sent, if at all.
		/// </summary>
		[FieldMapping("SentOn")]
		public DateTime SentOn
		{
			get { return _sentOn; }
			set { _sentOn = value; }
		}

		/// <summary>
		///		The <see cref="DateTime"/> that the email was queued, if it
		///		was queued.
		/// </summary>
		[FieldMapping("QueuedOn")]
		public DateTime QueuedOn
		{
			get { return _queuedOn; }
			set { _queuedOn = value; }
		}

		/// <summary>
		///		The <see cref="DateTime"/> that the last email attempt was made.
		/// </summary>
		[FieldMapping("LastTry")]
		public DateTime LastTry
		{
			get { return _lastTry; }
			set { _lastTry = value; }
		}

		/// <summary>
		///		The number of attempts that have been made to send this email.
		/// </summary
		[FieldMapping("NumberOfTries")]
		public int NumberOfTries
		{
			get { return _numberOfTries; }
			set { _numberOfTries = value; }
		}

		/// <summary>
		///		The text or HTML that goes in the body of the email
		/// </summary>
		[FieldMapping("Message")]
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		/// <summary>
		/// The user who's intended to receive this message.
		/// </summary>
		[FieldMapping("UserId")]
		public object UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		#endregion

		/// <summary>
		///		Gets a <see cref="MailMessage"/> that contains the values
		///		from this <see cref="EmailMessage"/>.
		/// </summary>
		/// <returns></returns>
		public MailMessage GetMailMessage()
		{
			MailMessage msg;

			msg = new MailMessage();

			//From
			if (_from == null || _from.Length == 0)
				throw new Exception("No From address available");

			msg.From = new MailAddress(_from);

			//To
			if (_toAddress == null || _toAddress.Length == 0)
				throw new Exception("No To address available");

			if (_toName == null || _toName.Length == 0)
				msg.To.Add(_toAddress);
			else
				msg.To.Add(string.Format("<{0}> {1}", _toAddress, _toName));

			//Subject
			msg.Subject = _subject;

			//Format
			msg.IsBodyHtml = _html;

			msg.Body = _message;

			return msg;
		}

		public void ApplyToUser(MembershipUser membershipUser, UserInformation userInformation)
		{
			ToAddress = membershipUser.Email;
			ToName = userInformation == null ? null : userInformation.FullName;
			Subject = ApplyToUser(Subject, membershipUser, userInformation);
			Message = ApplyToUser(Message, membershipUser, userInformation);
		}

		/// <summary>
		///		Searches the email message for the specified token and replaces
		///		it with the specified string.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="replacementString"></param>
		/// <remarks>
		///		Supports replacements in the Subject and Message Body.
		/// </remarks>
		public void ReplaceInMessage(string token, string replacementString)
		{
			Subject = ReplaceInString(Subject, token, replacementString);
			Message = ReplaceInString(Message, token, replacementString);
		}

		public void ReplaceGeneralTokens()
		{
			Subject = ReplaceGeneralTokens(Subject);
			Message = ReplaceGeneralTokens(Message);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string ReplaceGeneralTokens(string message)
		{
			message =
				ReplaceInString(message, TOKEN_HOMEPAGE_LINK,
				                "<a href=\"http://www.RankTrend.com/\" title=\"RankTrend.com\">RankTrend</a>");
			message = ReplaceInString(message, TOKEN_HOMEPAGE_URL, "http://www.RankTrend.com/");
			message =
				ReplaceInString(message, TOKEN_LOGO,
				                "<a href=\"http://www.RankTrend.com/\" title=\"RankTrend.com\"><img src=\"http://www.ranktrend.com/Images/RankTrend-Logo.gif\" alt=\"Rank Trend Logo\" style=\"border:none;\" /></a>");
			return message;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string ApplyToUser(string message, MembershipUser membershipUser, UserInformation userInformation)
		{
			message = ReplaceInString(message, TOKEN_USERUID, membershipUser.ProviderUserKey.ToString());
			message = ReplaceInString(message, TOKEN_EMAIL, membershipUser.Email);
			message = ReplaceInString(message, TOKEN_USERNAME, membershipUser.UserName);
			message =
				ReplaceInString(message, TOKEN_FIRSTNAME, userInformation == null ? membershipUser.Email : userInformation.FirstName);
			message =
				ReplaceInString(message, TOKEN_LASTNAME, userInformation == null ? membershipUser.Email : userInformation.LastName);
			message =
				ReplaceInString(message, TOKEN_FULLNAME, userInformation == null ? membershipUser.Email : userInformation.FullName);
			return message;
		}

		protected static string ReplaceInString(string searchString, string token, string replacementString)
		{
			if (searchString != null)
				return
					Regex.Replace(searchString, token, replacementString == null ? string.Empty : replacementString,
					              RegexOptions.IgnoreCase);
			else
				return null;
		}
	}
}