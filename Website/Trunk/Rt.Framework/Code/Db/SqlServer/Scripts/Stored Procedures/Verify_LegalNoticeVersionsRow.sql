if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Verify_LegalNoticeVersionsRow]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Verify_LegalNoticeVersionsRow]
GO

CREATE PROCEDURE dbo.Verify_LegalNoticeVersionsRow
	@Id Int,
	@LegalNoticeId int,
	@Notice ntext,
	@Timestamp datetime
AS

/*

Summary: Verifies the contents of the static data row in the LegalNoticeVersions
	table and makes it match the values specified.

*/

If Exists(Select [Id] From LegalNoticeVersions Where [Id] = @Id)
	Begin
		Update LegalNoticeVersions
		Set LegalNoticeId = @LegalNoticeId,
		Notice = @Notice,
		[Timestamp] = @Timestamp
		Where [Id] = @Id
	End
Else
	Begin
		Set Identity_Insert LegalNoticeVersions On

		Insert Into LegalNoticeVersions
		([Id], LegalNoticeId, Notice, [Timestamp])
		Values(@Id, @LegalNoticeId, @Notice, @Timestamp)

		Set Identity_Insert LegalNoticeVersions Off
	End

Go

GRANT EXECUTE ON dbo.Verify_LegalNoticeVersionsRow TO [Public]
Go

/*

sp_columns legalnoticeversions

*/