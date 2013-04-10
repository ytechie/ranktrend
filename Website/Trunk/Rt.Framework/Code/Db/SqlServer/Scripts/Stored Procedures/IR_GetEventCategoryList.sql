if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[IR_GetEventCategoryList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[IR_GetEventCategoryList]
Go

CREATE PROCEDURE dbo.[IR_GetEventCategoryList]
	@UserId UniqueIdentifier
AS

Select ec.Id, ec.[Name], Urls.Url
From rt_EventCategories ec
Join Urls On Urls.Id = ec.UrlId
Where Urls.UserId = @UserId
Order By Urls.Url, [Name]

Go

GRANT EXECUTE ON dbo.[IR_GetEventCategoryList] TO [Public]
Go

/*

Select *
From rt_EventCategories

select * from urls

exec IR_GetEventCategoryList '66F6A117-26EC-44D8-A35B-134D833ED5FC'
*/

