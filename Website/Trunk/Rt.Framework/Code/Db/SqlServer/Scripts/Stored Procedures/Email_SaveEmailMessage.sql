if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_SaveEmailMessage]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_SaveEmailMessage]
GO

CREATE PROCEDURE dbo.Email_SaveEmailMessage
	@EmailId Int = Null,
	@From Varchar(50),
	@ToName Varchar(50) = Null,
	@ToAddress Varchar(50),
	@Subject Varchar(100),
	@Message Text,
	@Html Bit,
	@SentOn DateTime = Null,
	@QueuedOn DateTime = Null,
	@LastTry DateTime = Null,
	@NumberOfTries Int = 0,
	@UserId UniqueIdentifier = Null
AS


If @EmailId Is Null
	Begin
		Insert Into EmailQueue
		([From], [ToName], [ToAddress], Subject, Message, [Html], SentOn, QueuedOn, LastTry, NumberOfTries, UserId)
		Values(@From, @ToName, @ToAddress, @Subject, @Message, @Html, @SentOn, GetUtcDate(), @LastTry, @NumberOfTries, @UserId)

		Return @@Identity
	End
Else
	Begin
		Update EmailQueue
		Set [From] = @From,
		ToName = @ToName,
		ToAddress = @ToAddress,
		Subject = @Subject,
		Message = @Message,
		[Html] = @Html,
		SentOn = @SentOn, 
		QueuedOn = @QueuedOn,
		LastTry = @LastTry,
		NumberOfTries = @NumberOfTries,
		UserId = @UserId
		Where [Id] = @EmailId

		Return @EmailId
	End


GO

GRANT EXECUTE ON dbo.Email_SaveEmailMessage TO [Public]
Go

/*
select * from emailqueue
sp_columns emailqueue
*/

