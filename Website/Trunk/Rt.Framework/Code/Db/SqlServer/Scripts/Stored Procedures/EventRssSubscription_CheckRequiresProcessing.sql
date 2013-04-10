if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[EventRssSubscription_CheckRequiresProcessing]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[EventRssSubscription_CheckRequiresProcessing]
GO

CREATE PROCEDURE dbo.EventRssSubscription_CheckRequiresProcessing
	@EventRssSubscriptionId Int,
	@RequiresProcessing Bit Output
AS

/*

Summary: 

*/

If Exists(Select * From rt_EventRssSubscriptions Where Id = @EventRssSubscriptionId And (LastCheck Is Null Or DateAdd(hh, 4, LastCheck) <= GetUTCDate()))
Begin
	Update rt_EventRssSubscriptions
		Set LastCheck = GetUTCDate()
	Where Id = @EventRssSubscriptionId
	
	Select @RequiresProcessing = 1
End
Else
	Select @RequiresProcessing = 0

Go

GRANT EXECUTE ON dbo.EventRssSubscription_CheckRequiresProcessing TO [Public]
Go

/*

sp_columns legalnoticeversions

*/