if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetNextConfiguredDatasourceForUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetNextConfiguredDatasourceForUser]
Go

CREATE PROCEDURE dbo.[Ds_GetNextConfiguredDatasourceForUser]
	@Guid UniqueIdentifier
AS

Declare @ConfiguredDatasourceId Int

Select @ConfiguredDatasourceId = cd.Id
From ConfiguredDatasources cd
Left Outer Join RawData rd On cd.Id = rd.ConfiguredDatasourceId
Join Urls On Urls.Id = cd.UrlId
--Calculate if we're in a new time period
Where (rd.[Timestamp] Is Null Or DateAdd(d, cd.CheckFrequencyDays, rd.[Timestamp]) < GetUtcDate())
--Wait at least an hour before reattempting to try this datasource
And (cd.LastCheckAttempt Is Null Or DateAdd(hh, 1, cd.LastCheckAttempt) < GetUtcDate())
And cd.Enabled = 1
And Urls.UserId = @Guid
Order By rd.[Timestamp] Desc

--Is this necessary?
--Order By DateAdd(d, cd.CheckFrequencyDays, rd.[Timestamp]) Asc

Update ConfiguredDatasources
Set LastCheckAttempt = GetUtcDate()
Where Id = @ConfiguredDatasourceId

Select *
From ConfiguredDatasources
Where Id = @ConfiguredDatasourceId

Go

GRANT EXECUTE ON dbo.[Ds_GetNextConfiguredDatasourceForUser] TO [Public]
Go

/*

exec Ds_GetNextConfiguredDatasourceForUser '66F6A117-26EC-44D8-A35B-134D833ED5FC'

select * from ConfiguredDatasources
select * from rawdata
select * from Urls
select * from userInformation
select * from aspnet_users
select * from aspnet_applications

*/

