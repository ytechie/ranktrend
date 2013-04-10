if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Legal_SaveNoticeVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Legal_SaveNoticeVersion]
GO

CREATE PROCEDURE dbo.Legal_SaveNoticeVersion
  @NoticeId Int,
	@Notice nText
AS

Insert Into LegalNoticeVersions (
	LegalNoticeId, Notice, Timestamp )
Values (
	@NoticeId, @Notice, GetUtcDate() )

Return @@Identity

GO

GRANT EXECUTE ON dbo.Legal_SaveNoticeVersion TO [Public]
Go


/*
Legal_SaveNoticeAgreement 1, 11, 1

select * from legalnoticeagreements



*/

