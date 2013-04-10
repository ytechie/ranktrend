using System;
using System.Collections.Generic;
using System.Web.Security;
using NHibernate;
using NHibernate.Expression;
using Rt.Framework.Components;
using Rt.Framework.Db.SqlServer;

/// <summary>
/// Summary description for TextMessagesInterface
/// </summary>
public static class TextMessagesInterface
{
	public static void SendMessage(Database db, MembershipUser user, string message)
	{
		SendMessage(db, (Guid) user.ProviderUserKey, message);
	}

	public static void SendMessage(Database db, Guid user, string message)
	{
		TextMessage textMessage = new TextMessage();
		textMessage.Message = message;
		textMessage.UserId = user;
		textMessage.Acknowledged = false;

		save(db, textMessage);
	}

	public static void AcknowledgeMessage(Database db, int id)
	{
		TextMessage textMessage;
		ISession session = db.ORManager.Session;
		try
		{
			textMessage = session.Get<TextMessage>(id);
		}
		finally
		{
			db.ORManager.CloseSession(session);
		}
		AcknowledgeMessage(db, textMessage);
	}

	public static void AcknowledgeMessage(Database db, TextMessage textMessage)
	{
		textMessage.Acknowledged = true;
		save(db, textMessage);
	}

	public static IList<TextMessage> GetMessages(Database db, MembershipUser user)
	{
		return GetMessages(db, (Guid) user.ProviderUserKey);
	}

	public static IList<TextMessage> GetMessages(Database db, Guid user)
	{
		IList<TextMessage> messages;
		ISession session = db.ORManager.Session;
		try
		{
			ICriteria criteria = session.CreateCriteria(typeof (TextMessage))
				.Add(Expression.Eq("UserId", user))
				.Add(Expression.Eq("Acknowledged", false))
				.AddOrder(Order.Asc("Timestamp"));
			messages = criteria.List<TextMessage>();
		}
		finally
		{
			db.ORManager.CloseSession(session);
		}

		return messages;
	}

	private static void save(Database db, TextMessage textMessage)
	{
		ISession session = db.ORManager.Session;
		try
		{
			session.SaveOrUpdate(textMessage);
			session.Flush();
		}
		finally
		{
			db.ORManager.CloseSession(session);
		}
	}
}