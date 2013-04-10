if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DataIntegrityEngine_RunCycle]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DataIntegrityEngine_RunCycle]
GO

CREATE PROCEDURE dbo.DataIntegrityEngine_RunCycle
AS

Declare @ThisApp UniqueIdentifier
Declare @UserCount Int
Declare @UserCountExpected Int

Select @ThisApp = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'

/*=============================================================*/
/* Delete inactive users */

Declare @DeleteUsers Table (
	UserId UniqueIdentifier )

Insert Into @DeleteUsers
	Select m.UserId	--, m.CreateDate, u.LastActivityDate
	From aspnet_membership m
	Join aspnet_users u On u.ApplicationId = m.ApplicationId And m.UserId = u.UserId
	Where m.ApplicationId = @ThisApp
		And m.IsApproved = 0
		And m.CreateDate <= DateAdd(m, -1, GetUtcDate())
		And u.LastActivityDate <= DateAdd(d, -15, GetUtcDate())
		And u.LastActivityDate != m.CreateDate

Select @UserCountExpected = Count(*) From aspnet_membership
Select @UserCountExpected = @UserCountExpected - Count(*) From @DeleteUsers

Begin Transaction

BEGIN TRY
	--Delete aspnet_membership
	Select *
	From aspnet_membership m
	Join @DeleteUsers du On m.UserId = du.UserId

	Select @UserCount = Count(*) From aspnet_membership

	If @UserCount != @UserCountExpected
		Rollback Transaction
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
	
/*=============================================================*/
/* Email unapproved users a second verify email */

Declare @UpdateUsers Table (
	UserId UniqueIdentifier )

Insert Into @UpdateUsers
	Select UserId --, CreateDate
	From aspnet_membership
	Where ApplicationId = @ThisApp
		And IsApproved = 0
		And CreateDate <= DateAdd(m, -1, GetUtcDate())

Go

GRANT EXECUTE ON dbo.DataIntegrityEngine_RunCycle TO [Public]
Go

/*

DataIntegrityEngine_RunCycle

*/