if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Users_GetUserInformation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Users_GetUserInformation]
GO

CREATE PROCEDURE dbo.Users_GetUserInformation
	@UserId UniqueIdentifier
AS

Select *
From UserInformation
Where UserId = @UserId

GO

GRANT EXECUTE ON dbo.Users_GetUserInformation TO [Public]
Go

/*

*/

