if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_DeleteDatasource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_DeleteDatasource]
Go

CREATE PROCEDURE dbo.[Ds_DeleteDatasource]
	@ConfiguredDatasourceId Int
AS

/*

Summary: Permanently deletes the datasource, all of its raw data,
	and all of the parameters.

*/

BEGIN TRY
	Begin Transaction

		Print 'Deleting all datasource parameters'
		Delete From DatasourceParameters
		Where ConfiguredDatasourceId = @ConfiguredDatasourceId

		Print 'Deleting raw data'
		Delete From RawData
		Where ConfiguredDatasourceId = @ConfiguredDatasourceId

		Print 'Deleting the datasource'
		Delete From ConfiguredDatasources
		Where Id = @ConfiguredDatasourceId
	
	Commit Transaction
END TRY
BEGIN CATCH
	Rollback Transaction

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    -- Use RAISERROR inside the CATCH block to return error
    -- information about the original error that caused
    -- execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;



Go

GRANT EXECUTE ON dbo.[Ds_DeleteDatasource] TO [Public]
Go

/*

select * from configuredDatasources
select * from DatasourceParameters
select * from rawdata

exec Ds_DeleteDatasource 12

*/

