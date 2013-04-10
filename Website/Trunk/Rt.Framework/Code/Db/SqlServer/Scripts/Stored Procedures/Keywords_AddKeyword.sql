if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Keywords_AddKeyword]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Keywords_AddKeyword]
Go

CREATE PROCEDURE dbo.[Keywords_AddKeyword]
	@UrlId Int,
	@Keyword Varchar(Max),
	@DatasourceTypeId Int
AS

--Check if the datasource exists
If Exists(
	Select *
	From ConfiguredDatasources cd
	Join DatasourceParameters dsp On cd.Id = dsp.ConfiguredDatasourceId
	Where dsp.ParameterNumber = 1
	And dsp.TextValue = @Keyword
	And cd.DatasourceTypeId = @DatasourceTypeId
	And cd.UrlId = @UrlId)
	Return 0



Begin Transaction

BEGIN TRY
	Declare @cdid Int

	--Insert the datasource
	--Print 'Inserting ' + @Keyword
	Insert Into ConfiguredDatasources
	(UrlId, DatasourceTypeId, [Name])
	Select @UrlId, @DatasourceTypeId, dt.Description + ' (' + @Keyword + ')'
	From DatasourceTypes dt
	Where dt.Id = @DatasourceTypeId

	Set @cdid = Scope_Identity()

	--Insert the parameter, which is the search phrase
	Insert Into DatasourceParameters
	(ParameterNumber, TextValue, ConfiguredDatasourceId)
	Values(1, @Keyword, @cdid)	

	Commit Transaction
END TRY
BEGIN CATCH
	Rollback Transaction

	Declare @ErrorMessage nvarchar(4000);
	Declare @ErrorSeverity int;
	Declare @ErrorState int;

	Select 
		@ErrorMessage = ERROR_MESSAGE(),
		@ErrorSeverity = ERROR_SEVERITY(),
		@ErrorState = ERROR_STATE();

	-- Use RAISERROR inside the CATCH block to return error
	-- information about the original error that caused
	-- execution to jump to the CATCH block.
	Raiserror (@ErrorMessage, -- Message text.
		@ErrorSeverity, -- Severity.
		@ErrorState -- State.
		)
END CATCH

Go

GRANT EXECUTE ON dbo.[Keywords_AddKeyword] TO [Public]
Go

/*

select top 100 * from configureddatasources
select * from configureddatasources
select * from datasourceparameters
select * from datasourcetypes
select * from urls



[Keywords_BulkImport] 1, '<keywords><keyword phrase="superjason"></keyword><keyword phrase="jason young"></keyword></keywords>', 1

[Keywords_AddKeyword] 1, 'young technologies', 1


*/