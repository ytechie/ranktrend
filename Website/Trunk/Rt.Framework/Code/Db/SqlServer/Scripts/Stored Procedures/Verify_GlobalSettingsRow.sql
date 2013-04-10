if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Verify_GlobalSettingsRow]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Verify_GlobalSettingsRow]
GO

CREATE PROCEDURE dbo.Verify_GlobalSettingsRow
	@Id Int,
	@Description Varchar(255),
	@IntValue Int,
	@TextValue Text
AS

/*

Summary: Verifies the contents of the static data row in the GlobalSettings
	table and makes it match the values specified.

*/

If Exists(Select [Id] From GlobalSettings Where [Id] = @Id)
	Begin
		Update GlobalSettings
		Set [Description] = @Description,
		IntValue = @IntValue,
		TextValue = @TextValue
		Where [Id] = @Id
	End
Else
	Begin
		Set Identity_Insert GlobalSettings On

		Insert Into GlobalSettings
		([Id], [Description], IntValue, TextValue)
		Values(@Id, @Description, @IntValue, @TextValue)

		Set Identity_Insert GlobalSettings Off
	End

GO

GRANT EXECUTE ON dbo.Verify_GlobalSettingsRow TO [Public]
Go

/*


*/