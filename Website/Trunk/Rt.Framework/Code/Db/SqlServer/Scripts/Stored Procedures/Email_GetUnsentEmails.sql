if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_GetUnsentEmails]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_GetUnsentEmails]
GO

CREATE PROCEDURE dbo.Email_GetUnsentEmails
AS

Select *
From EmailQueue
Where NumberOfTries <= 10
	And SentOn Is Null
Order By QueuedOn Asc, --Get oldest emails first
NumberOfTries Asc --Get the emails we haven't tried yet first

GO

GRANT EXECUTE ON dbo.Email_GetUnsentEmails TO [Public]
Go

/*

*/