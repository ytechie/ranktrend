if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ControlPanel_GetItemExclamations]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[ControlPanel_GetItemExclamations]
Go

CREATE PROCEDURE dbo.[ControlPanel_GetItemExclamations]
	@UserId UniqueIdentifier
AS

/*

Summary: Gets a list of bit statuses that determine if certain items on the control
	panel should have exclamation points by them, because they need to be completed.

*/

--Figure out if the user has any sites
Select 0 ItemNum, 'Has no sites' Description,
	Cast(Case When Count(*) = 0 Then 1 Else 0 End As Bit) ShowExclamation
From Urls
Where UserId = @UserId

Union All --Don't check for duplicates, there will be none

--Figure out if the user has any datasources
Select 1 ItemNum, 'Has no datasources' Description,
	Cast(Case When Count(*) = 0 Then 1 Else 0 End As Bit) ShowExclamation
From Urls u
Join ConfiguredDatasources cd On cd.UrlId = u.Id
Where u.UserId = @UserId

Union All

--Figure out if the user has any data collected, so we can guess if the tray app is working
Select 2 ItemNum, 'Has not downloaded tray app' Description,
	Cast(Case When Count(*) = 0 Then 1 Else 0 End As Bit) ShowExclamation
From Urls u
Join ConfiguredDatasources cd On cd.UrlId = u.Id
Join RawData rd On rd.ConfiguredDatasourceId = cd.Id
Where u.UserId = @UserId

Go

GRANT EXECUTE ON dbo.[ControlPanel_GetItemExclamations] TO [Public]
Go

/*

exec [ControlPanel_GetItemExclamations] '66F6A117-26EC-44D8-A35B-134D833ED5FC'
exec [ControlPanel_GetItemExclamations] 'E8EA5304-05E6-4A64-AA43-4DF3BB2527D3'

- Site configuration section
- Add datasource wizard

select * from urls
select * from aspnet_membership
select * from configureddatasources
select top 100 * from rawdata

*/

