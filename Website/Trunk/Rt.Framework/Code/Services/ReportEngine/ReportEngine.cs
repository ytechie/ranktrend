using System;
using System.Collections.Generic;
using System.Text;
using Rt.Framework.Db.SqlServer;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using Rt.Framework.Components;
using NHibernate.Expression;
using NHibernate;
using System.Web.Security;
using System.Reflection;

namespace Rt.Framework.Services.ReportEngine
{
	public class ReportEngine : RtEngineBase
	{
		public const int SERVICEID = 3;
		protected const string TOKEN_REPORT_CONTENT = "{REPORT_CONTENT}";
		protected const string TOKEN_REPORT_TITLE = "{REPORT_TITLE}";
		protected const string CID_REPORT_IMAGE = "reportimage";
		private EmailTemplate _customReportTemplate;

		public ReportEngine(Database db)
			: base(db)
		{
		}

		public override int ServiceId { get { return SERVICEID; } }

		protected override void ReloadConfiguration()
		{
			base.ReloadConfiguration();

			if (_db != null)
			{
				int emailTemplateId = (int)_db.GetGlobalSetting(10).IntValue;
				_customReportTemplate = _db.GetEmailTemplate(emailTemplateId);
			}
		}

		protected override void RunPreCycle()
		{
			
		}

		protected override void RunCycle()
		{
			int? reportId = _db.GetNextCustomReportId();
			while (reportId != null)
			{
				try
				{
					_log.DebugFormat("{0}: Next custom report to send is {1}.", _name, reportId);
					SendReport((int)reportId);
				}
				catch (Exception ex)
				{
					_log.Error(string.Format("{0}: Error sending report {1}.", _name, reportId), ex);

					CustomReport report = _db.ORManager.Get<CustomReport>(reportId);
					if (report.LastEmailed != null)
					{
						_log.DebugFormat("{0}: Resetting custom report {1} so it is processed again next service cycle.", _name, reportId);
						// Reset the failed report so that it runs next time the engine runs
						// Add _cycleInterval so that it this report does not get run until the next service cycle
						report.LastEmailed = ((DateTime)report.LastEmailed).AddDays(-report.EmailIntervalDays).AddMilliseconds(_cycleInterval);
						_db.ORManager.SaveOrUpdate(report);
					}
				}

				reportId = _db.GetNextCustomReportId();
			}

			_log.DebugFormat("{0}: No more custom reports to process.", _name);
		}

		public virtual void SendReport(int id)
		{
			ReportPartGenerator rpg = new ReportPartGenerator(_db);
			EmailMessage email = new EmailMessage(_customReportTemplate);
			StringBuilder htmlBuilder = new StringBuilder();
			Dictionary<string, MemoryStream> imageStreams = new Dictionary<string,MemoryStream>();
			int imgCounter = 0;

			CustomReport report = _db.ORManager.Get<CustomReport>(id);

			// Prepare the EmailMessage
			email.From = "Support@RankTrend.com";		// Placeholder - will be replaced by email engine
			email.ReplaceGeneralTokens();
			email.ApplyToUser(Membership.GetUser(report.UserId), _db.ORManager.Get<UserInformation>(report.UserId));
			email.ReplaceInMessage(TOKEN_REPORT_TITLE, report.Name);

			// Fetch the custom report components
			ICriteria componentCriteria = _db.ORManager.Session.CreateCriteria(typeof(CustomReportComponent)).Add(Expression.Eq("CustomReport.Id", id));
			IList<CustomReportComponent> components = componentCriteria.List<CustomReportComponent>();

			htmlBuilder.Append("<table class=\"ReportComponentsTable\">");

			// Add each custom report component to the custom report
			foreach(CustomReportComponent component in components)
			{
				GeneratedReportPart grp = rpg.GenerateReport(component);
				if(grp.Bytes != null)
				{
					// Create a placeholder for the embedded image
					string cid = string.Format("{0}{1}", CID_REPORT_IMAGE, imgCounter++);

					// Create an embedded image tag
					htmlBuilder.AppendFormat("<tr><td><img src=cid:{0}><br /></td></tr>", cid);

					// Generate and save the memory stream for this embeded image
					MemoryStream ms = new MemoryStream(grp.Bytes);
					imageStreams.Add(cid, ms);
				}
				else
				{
					// Append the raw HTML
					htmlBuilder.AppendFormat("<tr><td>{0}</td></tr>", grp.Html);
				}
			}

			htmlBuilder.Append("</table>");

			// Insert report components into email message
			email.ReplaceInMessage(TOKEN_REPORT_CONTENT, htmlBuilder.ToString());
			MailMessage msg = email.GetMailMessage();

			// Create Text-Only AlternateView
			AlternateView plainView = AlternateView.CreateAlternateViewFromString("Your email client does not appear to support viewing HTML emails.  Your email client must support this in order to view this email.", Encoding.Default, "text/plain");
			msg.AlternateViews.Add(plainView);
			
			// Prepare new AlternateView and clear the message body
			AlternateView htmlView = AlternateView.CreateAlternateViewFromString(msg.Body, Encoding.Default, MediaTypeNames.Text.Html);
			htmlView.TransferEncoding = TransferEncoding.SevenBit;
			msg.AlternateViews.Add(htmlView);
			msg.Body = null;

			// Embed Logo
			EmbedGifImage(htmlView, GetEmbeddedImage("RankTrend-Logo.gif"), "EmbeddedLogo");

			// Embed the images
			foreach (string key in imageStreams.Keys)
				EmbedPngImage(htmlView, imageStreams[key], key);

			// Send the email
			RtEngines.EmailEngine.SendEmail(msg, true);
		}

		protected virtual Stream GetEmbeddedImage(string name)
		{
			Assembly a = typeof(ReportEngine).Assembly;
			return a.GetManifestResourceStream("Rt.Framework.Services.ReportEngine.EmbeddedImages." + name);
		}

		protected virtual void EmbedPngImage(AlternateView htmlView, Stream image, string contentId)
		{
			EmbedImage(htmlView, image, contentId, "png");
		}

		protected virtual void EmbedGifImage(AlternateView htmlView, Stream image, string contentId)
		{
			EmbedImage(htmlView, image, contentId, "gif");
		}

		protected virtual void EmbedImage(AlternateView htmlView, Stream image, string contentId, string type)
		{
			// Create and add the embedded image
			LinkedResource linkedResource = new LinkedResource(image, "image/" + type);
			linkedResource.ContentId = contentId;
			linkedResource.TransferEncoding = TransferEncoding.Base64;
			htmlView.LinkedResources.Add(linkedResource);
		}
	}
}
