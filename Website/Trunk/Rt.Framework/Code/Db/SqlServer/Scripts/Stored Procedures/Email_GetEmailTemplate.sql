if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_GetEmailTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_GetEmailTemplate]
GO

CREATE PROCEDURE dbo.Email_GetEmailTemplate
	@Id Int
AS

Select *
From EmailTemplates
Where Id = @Id

GO

GRANT EXECUTE ON dbo.Email_GetEmailTemplate TO [Public]
Go

/*

*/

