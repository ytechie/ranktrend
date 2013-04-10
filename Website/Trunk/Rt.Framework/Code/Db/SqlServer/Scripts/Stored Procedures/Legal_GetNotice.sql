if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Legal_GetNotice]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Legal_GetNotice]
GO

CREATE PROCEDURE dbo.Legal_GetNotice
  @Id Int
AS

Select *
From LegalNotices
Where Id = @Id

GO

GRANT EXECUTE ON dbo.Legal_GetNotice TO [Public]
Go


/*
Legal_GetNoticeAgreement 1, 11

Insert Into LegalNoticeVersions (
	LegalNoticeId, Notice )
Values (
	1, 'Placeholder for Affiliate Terms of Service.' )

*/

