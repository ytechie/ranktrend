if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SavedReports_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[SavedReports_Delete]
Go

CREATE PROCEDURE dbo.[SavedReports_Delete]
	@SavedReportId Int
AS

/*

Summary: Permanently deletes the saved report.

*/

BEGIN TRY
	Begin Transaction

		Print 'Removing the saved report from all custom reports'
		Delete From rt_CustomReportComponents
		Where SavedReportId = @SavedReportId
		
		Print 'Deleting the saved report'
		Delete From rt_SavedReports
		Where Id = @SavedReportId
	
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

GRANT EXECUTE ON dbo.[SavedReports_Delete] TO [Public]
Go

/*

select * from rt_savedreports
select * from rt_customreportcomponents

*/

