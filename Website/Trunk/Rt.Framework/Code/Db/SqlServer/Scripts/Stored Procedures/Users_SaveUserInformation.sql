if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Users_SaveUserInformation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Users_SaveUserInformation]
GO

CREATE PROCEDURE dbo.Users_SaveUserInformation
	@UserId UniqueIdentifier,
	@FirstName nvarchar(256),
	@LastName nvarchar(256)
AS

If Not Exists(Select * From UserInformation Where UserId = @UserId)
	Begin
		Insert Into UserInformation
		( UserId, FirstName, LastName )
		Values
		( @UserId, @FirstName, @LastName )
	End
Else
	Begin
		Update UserInformation
		Set
			FirstName = @FirstName,
			LastName = @LastName
		Where UserId = @UserId
	End


GO

GRANT EXECUTE ON dbo.Users_SaveUserInformation TO [Public]
Go

/*

*/

