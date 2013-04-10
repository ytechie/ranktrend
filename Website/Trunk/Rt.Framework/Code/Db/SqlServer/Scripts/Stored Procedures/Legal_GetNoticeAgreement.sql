if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Legal_GetNoticeAgreement]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Legal_GetNoticeAgreement]
GO

CREATE PROCEDURE dbo.Legal_GetNoticeAgreement
  @NoticeVersionId Int,
	@UserId UniqueIdentifier
AS

Select Top 1 *
From LegalNoticeAgreements
Where LegalNoticeVersionId = @NoticeVersionId And UserId = @UserId
Order By Id Desc

GO

GRANT EXECUTE ON dbo.Legal_GetNoticeAgreement TO [Public]
Go


/*
Legal_GetNoticeAgreement 1, 11

Insert Into LegalNoticeVersions (
	LegalNoticeId, Notice )
Values (
	1, 'Placeholder for Affiliate Terms of Service.' )

*/

