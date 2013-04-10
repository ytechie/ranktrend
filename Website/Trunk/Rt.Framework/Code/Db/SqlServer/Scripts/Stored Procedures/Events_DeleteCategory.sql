if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Events_DeleteCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Events_DeleteCategory]
Go

CREATE PROCEDURE dbo.[Events_DeleteCategory]
	@EventCategoryId Int
AS

/*

Summary: Permanently deletes the event category and all of the associated events.

*/

BEGIN TRY
	Begin Transaction

		Print 'Redirecting subscriptions to the default event category'
		Update rt_EventRssSubscriptions
		Set EventCategoryId = Null
		Where EventCategoryId = @EventCategoryId

		Print 'Deleting all associated events'
		Delete From rt_Events
		Where CategoryId = @EventCategoryId

		Print 'Deleting the category'
		Delete From rt_EventCategories
		Where Id = @EventCategoryId
	
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

GRANT EXECUTE ON dbo.[Events_DeleteCategory] TO [Public]
Go

/*

select * from rt_events
select * from rt_eventcategories
select * from rt_eventrsssubscriptions

*/

