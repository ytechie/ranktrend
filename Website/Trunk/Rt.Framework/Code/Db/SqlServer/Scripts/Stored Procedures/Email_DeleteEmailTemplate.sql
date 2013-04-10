if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_DeleteEmailTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_DeleteEmailTemplate]
GO

CREATE PROCEDURE dbo.Email_DeleteEmailTemplate
	@EmailTemplateId Int
AS

Delete
From EmailTemplates
Where Id = @EmailTemplateId And Locked = 0

GO

GRANT EXECUTE ON dbo.Email_DeleteEmailTemplate TO [Public]
Go

/*
select * from emailqueue
sp_columns emailqueue
*/

