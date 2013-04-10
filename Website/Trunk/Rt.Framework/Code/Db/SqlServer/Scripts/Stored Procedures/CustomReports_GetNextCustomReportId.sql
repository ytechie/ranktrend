if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CustomReports_GetNextCustomReportId]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[CustomReports_GetNextCustomReportId]
Go

CREATE PROCEDURE dbo.[CustomReports_GetNextCustomReportId]
AS

/*

Summary: 

Remarks: 

*/

Begin Transaction

BEGIN TRY
	Declare @CustomReportId Int

	Select Top 1 @CustomReportId = Id
	From rt_CustomReports
	Where EmailIntervalDays > 0
		And (LastEmailed Is Null Or DateAdd(d, EmailIntervalDays, LastEmailed) < GetUtcDate())
	Order By LastEmailed Asc, Id Asc

	Update rt_CustomReports
	Set
		LastEmailed = GetUtcDate()
	Where Id = @CustomReportId
END TRY
BEGIN CATCH
	If @@Trancount > 0
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
             );
END CATCH;

If @@Trancount > 0
    Commit Transaction;

Select @CustomReportId

Go

GRANT EXECUTE ON dbo.[CustomReports_GetNextCustomReportId] TO [Public]
Go

/*
CustomReports_GetNextCustomReportId

select * from rt_customreports
*/

