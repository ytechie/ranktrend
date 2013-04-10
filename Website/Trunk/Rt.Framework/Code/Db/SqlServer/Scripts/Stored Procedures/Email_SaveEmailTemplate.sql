if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_SaveEmailTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_SaveEmailTemplate]
GO

CREATE PROCEDURE dbo.Email_SaveEmailTemplate
	@EmailTemplateId Int = Null,
	@Subject Varchar(100),
	@Message Text,
	@Html Bit,
	@Locked Bit
AS


If @EmailTemplateId Is Null
	Begin
		Insert Into EmailTemplates
		(Subject, Message, [Html], Locked)
		Values(@Subject, @Message, @Html, @Locked)

		Return @@Identity
	End
Else
	Begin
		Update EmailTemplates
		Set Subject = @Subject,
		Message = @Message,
		[Html] = @Html,
		Locked = @Locked
		Where [Id] = @EmailTemplateId

		Return @EmailTemplateId
	End


GO

GRANT EXECUTE ON dbo.Email_SaveEmailMessage TO [Public]
Go

/*
select * from emailqueue
sp_columns emailqueue
*/

