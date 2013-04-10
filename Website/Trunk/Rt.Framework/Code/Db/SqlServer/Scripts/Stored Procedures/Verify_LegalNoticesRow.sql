if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Verify_LegalNoticesRow]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Verify_LegalNoticesRow]
GO

CREATE PROCEDURE dbo.Verify_LegalNoticesRow
	@Id Int,
	@Description NVarchar(255)
AS

/*

Summary: Verifies the contents of the static data row in the LegalNotices
	table and makes it match the values specified.

*/

If Exists(Select [Id] From LegalNotices Where [Id] = @Id)
	Begin
		Update LegalNotices
		Set [Description] = @Description
		Where [Id] = @Id
	End
Else
	Begin
		Set Identity_Insert LegalNotices On

		Insert Into LegalNotices
		([Id], [Description])
		Values(@Id, @Description)

		Set Identity_Insert LegalNotices Off
	End

Go

GRANT EXECUTE ON dbo.Verify_LegalNoticesRow TO [Public]
Go

/*

sp_columns legalnotices

*/