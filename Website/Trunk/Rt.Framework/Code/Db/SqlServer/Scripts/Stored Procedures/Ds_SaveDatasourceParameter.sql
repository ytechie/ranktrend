if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_SaveDatasourceParameter]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Ds_SaveDatasourceParameter]
GO

CREATE PROCEDURE dbo.[Ds_SaveDatasourceParameter]
	@Id Int = Null Output,
	@ParameterNumber Int,
	@IntValue Int,
	@TextValue Varchar(Max),
	@ConfiguredDatasourceId Int
AS

If @Id Is Null
	Begin
		Insert Into DatasourceParameters
		(ParameterNumber, IntValue, TextValue, ConfiguredDatasourceId)
		Values
		(@ParameterNumber, @IntValue, @TextValue, @ConfiguredDatasourceId)

		Set @Id = @@Identity
	End
Else
	Begin
		Update DatasourceParameters
		Set
			ParameterNumber = @ParameterNumber,
			IntValue = @IntValue,
			TextValue = @TextValue,
			ConfiguredDatasourceId = @ConfiguredDatasourceId
		Where Id = @Id
	End

GO

GRANT EXECUTE ON dbo.[Ds_SaveDatasourceParameter] TO [Public]
Go

/*
select * from datasourceparameters
sp_columns datasourceparameters
*/

