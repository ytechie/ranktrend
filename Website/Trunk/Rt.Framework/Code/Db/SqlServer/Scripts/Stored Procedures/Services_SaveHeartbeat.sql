if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Services_SaveHeartbeat]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Services_SaveHeartbeat]
Go

CREATE PROCEDURE dbo.[Services_SaveHeartbeat]
  @ServiceId Int,
	@ReloadConfiguration Bit Output,
	@ForceRun Bit Output
AS

/*

Summary: Updates the last heartbeat time for a service

*/

Select @ReloadConfiguration = ReloadConfiguration,
	@ForceRun = ForceRun
From Services
Where Id = @ServiceId

Update Services
Set LastHeartbeat = GetUtcDate(),
ReloadConfiguration = 0,
ForceRun = 0
Where Id = @ServiceId

Go

GRANT EXECUTE ON dbo.[Services_SaveHeartbeat] TO [Public]
Go

/*
select * from services

Declare @Reload bit
exec Services_SaveHeartbeat 1, @Reload output

print @Reload
*/

