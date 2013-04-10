if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_SaveConfiguredDatasource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Ds_SaveConfiguredDatasource]
GO

CREATE PROCEDURE dbo.[Ds_SaveConfiguredDatasource]
	@Id Int = Null Output,
	@UrlId Int,
	@DatasourceTypeId Int,
	@CheckFrequencyDays Int,
	@Enabled Bit,
	@LastCheckAttempt DateTime = Null
AS

If @Id Is Null
	Begin
		Insert Into ConfiguredDatasources
		(UrlId, DatasourceTypeId, CheckFrequencyDays, Enabled, LastCheckAttempt)
		Values
		(@UrlId, @DatasourceTypeId, @CheckFrequencyDays, @Enabled, @LastCheckAttempt)

		Set @Id = @@Identity
	End
Else
	Begin
		Update ConfiguredDatasources
		Set
			UrlId = @UrlId,
			DatasourceTypeId = @DatasourceTypeId,
			CheckFrequencyDays = @CheckFrequencyDays,
			Enabled = @Enabled,
			LastCheckAttempt = @LastCheckAttempt
		Where Id = @Id
	End

GO

GRANT EXECUTE ON dbo.[Ds_SaveConfiguredDatasource] TO [Public]
Go

/*
select * from configureddatasources
*/

