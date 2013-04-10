if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Services_SetLastRunTime]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Services_SetLastRunTime]
Go

CREATE PROCEDURE dbo.[Services_SetLastRunTime]
  @ServiceId Int,
	@ReloadConfiguration Bit Output,
	@ForceRun Bit Output
AS

/*

Summary: Updates the poll time of a service.

*/

Select @ReloadConfiguration = ReloadConfiguration,
@ForceRun = ForceRun
From Services
Where Id = @ServiceId

Update Services
Set LastRunTime = GetUtcDate(),
ReloadConfiguration = 0,
ForceRun = 0
Where Id = @ServiceId

Go

GRANT EXECUTE ON dbo.[Services_SetLastRunTime] TO [Public]
Go

/*
select * from services

*/