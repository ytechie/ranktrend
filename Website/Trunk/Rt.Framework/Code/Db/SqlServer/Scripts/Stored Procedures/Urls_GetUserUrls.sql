if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Urls_GetUserUrls]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Urls_GetUserUrls]
Go

CREATE PROCEDURE dbo.[Urls_GetUserUrls]
	@UserId UniqueIdentifier
AS

/*

Summary: 

*/

Select *
From Urls
Where @UserId = UserId
Order By Url

Go

GRANT EXECUTE ON dbo.[Urls_GetUserUrls] TO [Public]
Go

/*

[Urls_GetUserUrls] '66F6A117-26EC-44D8-A35B-134D833ED5FC'

*/

