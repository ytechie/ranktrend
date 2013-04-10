using System.Web.Security;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;

namespace Rt.Framework.Applications
{
	public class EmailQueue
	{
		public static void EnqueueEmail(Database db, EmailMessage msg)
		{
			db.SaveEmail(msg);
		}

		public static void EnqueueEmail(Database db, EmailTemplate template, MembershipUser user, string from)
		{
			EmailMessage msg = new EmailMessage(template);
			msg.From = from;
			msg.ApplyToUser(user, db.ORManager.Get<UserInformation>(user.ProviderUserKey));
			db.SaveEmail(msg);
		}
	}
}