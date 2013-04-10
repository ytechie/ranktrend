if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Services_SaveService]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Services_SaveService]
Go

CREATE PROCEDURE dbo.[Services_SaveService]
  @ServiceId Int,
	@Description Varchar(Max),
	@RunIntervalMinutes Int,
	@Enabled Bit,
	@ReloadConfiguration Bit,
	@ForceRun Bit,
	@Owner nVarChar(50)
AS

/*

Summary: Saves the service information back to the database.

*/

If @ServiceId Is Null
	Begin
		Insert Into Services
		(Id, Description, LastHeartbeat, RunIntervalMinutes, Enabled, LastRunTime, ReloadConfiguration, ForceRun, Owner)
		Values(@ServiceId, @Description, null, @RunIntervalMinutes, @Enabled, null, @ReloadConfiguration, @ForceRun, @Owner)
	
		Return @@Identity
	End
Else
	Begin
	  Update Services
	  Set Description = @Description,
		RunIntervalMinutes = @RunIntervalMinutes,
		Enabled = @Enabled,
		ReloadConfiguration = @ReloadConfiguration,
		ForceRun = @ForceRun,
		Owner = @Owner
		Where Id = @ServiceId

		Return @ServiceId
	End

Go

GRANT EXECUTE ON dbo.[Services_SaveService] TO [Public]
Go

/*
select * from services

[Services_GetService] 1

sp_columns services
*/

