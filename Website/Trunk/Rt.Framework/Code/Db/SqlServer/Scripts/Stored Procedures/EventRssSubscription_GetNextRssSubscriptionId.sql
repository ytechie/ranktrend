if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[EventRssSubscription_GetNextRssSubscriptionId]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[EventRssSubscription_GetNextRssSubscriptionId]
GO

CREATE PROCEDURE dbo.EventRssSubscription_GetNextRssSubscriptionId
AS

/*

Summary: 

*/

Begin Transaction

BEGIN TRY
	Declare @RssSubscriptionid Int

	Select @RssSubscriptionId = Id
	From rt_EventRssSubscriptions
	Where ErrorCount < 10
		And (LastCheck Is Null Or DateAdd(hh, 4, LastCheck) <= GetUTCDate())
	Order By LastCheck Asc, Id Asc

	Update rt_EventRssSubscriptions
		Set LastCheck = GetUTCDate()
	Where Id = @RssSubscriptionid
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

Select @RssSubscriptionId

Go

GRANT EXECUTE ON dbo.EventRssSubscription_GetNextRssSubscriptionId TO [Public]
Go

/*

EventRssSubscription_GetNextRssSubscriptionId

*/