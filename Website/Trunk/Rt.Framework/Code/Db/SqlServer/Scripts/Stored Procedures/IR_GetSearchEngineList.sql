if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[IR_GetSearchEngineList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[IR_GetSearchEngineList]
Go

CREATE PROCEDURE dbo.[IR_GetSearchEngineList]
	@UrlId Int,
	@UserId UniqueIdentifier
AS

/*

Summary: Gets a list of search engines that have datasources configured
	for a specified page, for a specific user.

*/

Select Distinct se.Id, se.[Name]
From ConfiguredDatasources cd
Join Urls On cd.UrlId = Urls.Id
Join DatasourceTypes dt On cd.DatasourceTypeId = dt.Id
Left Outer Join SearchEngines se On dt.SearchEngineId = se.Id
Where (UrlId = @UrlId Or @UrlId Is Null)
And (Urls.UserId = @UserId Or @UserId Is Null)

Go

GRANT EXECUTE ON dbo.[IR_GetSearchEngineList] TO [Public]
Go

/*

[IR_GetSearchEngineList] NULL, NULL
[IR_GetSearchEngineList] 1, NULL
[IR_GetSearchEngineList] 1, '66F6A117-26EC-44D8-A35B-134D833ED5FC'
[IR_GetSearchEngineList] 2, NULL
select * from ConfiguredDatasources
select * from rawdata
select * from urls

*/

