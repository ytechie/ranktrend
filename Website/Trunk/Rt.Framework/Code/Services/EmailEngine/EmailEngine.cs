using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Db.SqlServer;
using Rt.Framework.Components;
using System.Net.Mail;
using System.Web.Security;

namespace Rt.Framework.Services.EmailEngine
{
	public class EmailEngine : RtEngineBase
	{
		public const int SERVICEID = 1;
		private string _mailServer;
		private string _fromEmail;
		private string _username;
		private string _password;
		private SmtpClient _smtpClient;

		public EmailEngine(Database db)
			: base(db)
		{
		}

		public override int ServiceId { get { return SERVICEID; } }

		/// <summary>
		///     Inherently reloads the configuration from the database.
		///     This includes the service information and resets
		///     the cycle time interval on the base class.
		/// </summary>
		/// <remarks>
		///     Overridden here to grab the latest Global Settings
		///     for the mail server and From email address.
		/// </remarks>
		protected override void ReloadConfiguration()
		{
			base.ReloadConfiguration();

			if (_db != null)
			{
				_fromEmail = _db.GetGlobalSetting(2).TextValue;
				_mailServer = _db.GetGlobalSetting(4).TextValue;
				_username = _db.GetGlobalSetting(8).TextValue;
				_password = _db.GetGlobalSetting(9).TextValue;

				_log.DebugFormat("{0}: Loaded 'From'' address from database: '{1}'", _name, _fromEmail);
				_log.DebugFormat("{0}: Loaded mail server address from database: '{1}'", _name, _mailServer);
				_log.DebugFormat("{0}: Loaded mail server credentials from database: '{1}':*****", _name, _username);

				_log.DebugFormat("{0}: Initializing SMTP Client.", _name);
				_smtpClient = new SmtpClient(_mailServer);
				_smtpClient.Credentials = new System.Net.NetworkCredential(_username, _password);
			}
		}

		protected override void RunPreCycle()
		{
		}

		/// <summary>
		///     Cycles through the email queue sending each email until
		///     there are no more emails to send.
		/// </summary>
		protected override void RunCycle()
		{
			EmailMessage queuedMessage;
			MailMessage msg;
			DateTime serverTime;
			bool sendFailure;
			MembershipUser user;
			UserInformation userInfo;

			_log.DebugFormat("{0}: Polling for a queued email.", _name);

			//Load the messages one at a time, send them, and mark them as processed.
			queuedMessage = _db.GetNextQueuedEmail();
			if (queuedMessage == null)
			{
				_log.DebugFormat("{0}: No messages found in the queue that meet the requirements.", _name);
				//there are no messages to send
				return;
			}

			while (queuedMessage != null)
			{
				//Get the date from the database server
				serverTime = _db.GetServerTime();

				//Update the From address
				queuedMessage.From = _fromEmail;

				sendFailure = false;

				try
				{
					if (queuedMessage.UserId != null)
					{
						_log.DebugFormat("{0}: Formatting the email message for user {1}.", _name, queuedMessage.UserId);
						user = Membership.GetUser(queuedMessage.UserId);
						userInfo = _db.ORManager.Get<UserInformation>(user.ProviderUserKey);
						queuedMessage.ReplaceGeneralTokens();
						queuedMessage.ApplyToUser(user, userInfo);
					}

					//Get the MailMessage object to send
					msg = queuedMessage.GetMailMessage();

					_log.DebugFormat("{0}: About to send the message.", _name);
					SendEmail(msg);
				}
				catch (Exception ex)
				{
					_log.Error(string.Format("{0}: There was an error sending the message.", _name), ex);

					queuedMessage.LastTry = serverTime;
					queuedMessage.NumberOfTries++;

					sendFailure = true;

					//Note: we don't need to return here, because this message should not
					//show up in the list of emails that needs to be sent for a while.
				}
				finally
				{
					//Put in an artificial delay so that we don't send out emails too fast
					System.Threading.Thread.Sleep(3000);
				}

				if (!sendFailure)
				{
					queuedMessage.SentOn = serverTime;
				}

				//In any case, we need to save the email with the new information
				_db.SaveEmail(queuedMessage);

				//Since we found an email the last time, lets check to see if there is another
				queuedMessage = _db.GetNextQueuedEmail();
			}
		}

		/// <summary>
		///		Attempts an immediate send of the given email message.
		/// </summary>
		/// <param name="message">
		///		The email message to send.
		/// </param>
		public void SendEmail(MailMessage message)
		{
			SendEmail(message, false);
		}

		/// <summary>
		///		Attempts an immediate send of the given email message.
		/// </summary>
		/// <param name="message">
		///		The email message to send.
		/// </param>
		/// <param name="updateFromAddress">
		///		If true, the support email address, as defined in the Global Settings,
		///		will be set as the From address of the <see cref="MailMessage"/> object.
		/// </param>
		public void SendEmail(MailMessage message, bool updateFromAddress)
		{
			if (updateFromAddress) message.From = new MailAddress(_fromEmail);
			_smtpClient.Send(message);
		}
	}
}
