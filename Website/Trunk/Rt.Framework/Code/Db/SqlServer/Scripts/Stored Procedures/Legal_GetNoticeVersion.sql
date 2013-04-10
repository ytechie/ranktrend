if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Legal_GetNoticeVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Legal_GetNoticeVersion]
GO

CREATE PROCEDURE dbo.Legal_GetNoticeVersion
  @NoticeId Int
AS

Select Top 1 *
From LegalNoticeVersions
Where LegalNoticeId = @NoticeId
Order By Id Desc

GO

GRANT EXECUTE ON dbo.Legal_GetNoticeVersion TO [Public]
Go


/*
Legal_GetNoticeVersion 1

Insert Into LegalNoticeVersions (
	LegalNoticeId, Notice )
Values (
	1, 'Placeholder for Affiliate Terms of Service.' )

*/