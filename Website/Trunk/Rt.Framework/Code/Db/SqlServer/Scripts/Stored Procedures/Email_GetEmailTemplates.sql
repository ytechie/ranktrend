if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_GetEmailTemplates]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_GetEmailTemplates]
GO

CREATE PROCEDURE dbo.Email_GetEmailTemplates
AS

Select *
From EmailTemplates

GO

GRANT EXECUTE ON dbo.Email_GetEmailTemplates TO [Public]
Go

/*

*/

