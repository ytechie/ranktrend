if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Legal_SaveNoticeAgreement]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Legal_SaveNoticeAgreement]
GO

CREATE PROCEDURE dbo.Legal_SaveNoticeAgreement
  @NoticeVersionId Int,
	@UserId UniqueIdentifier,
	@Agree Bit
AS

Declare @PreviousAgreement Bit

Select Top 1 @PreviousAgreement = a.Agree
From LegalNoticeAgreements a
Where LegalNoticeVersionId = @NoticeVersionId And UserId = @UserId
Order By Timestamp Desc

If @PreviousAgreement Is Null Or @PreviousAgreement <> @Agree
	Insert Into LegalNoticeAgreements (
		LegalNoticeVersionId, UserId, Agree, [Timestamp] )
	Values (
		@NoticeVersionId, @UserId, @Agree, GetUtcDate() )

GO

GRANT EXECUTE ON dbo.Legal_SaveNoticeAgreement TO [Public]
Go


/*
Legal_SaveNoticeAgreement 1, 11, 1

select * from legalnoticeagreements



*/

