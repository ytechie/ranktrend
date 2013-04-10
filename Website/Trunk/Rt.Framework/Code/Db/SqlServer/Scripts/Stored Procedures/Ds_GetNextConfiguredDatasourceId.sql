if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetNextConfiguredDatasourceId]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetNextConfiguredDatasourceId]
Go

CREATE PROCEDURE dbo.[Ds_GetNextConfiguredDatasourceId]
	@UserId As UniqueIdentifier = Null
AS

/*

Summary: Gets the configured data source ID that is most due for an update.  If no
					datasources are due for an update, no rows are returned.

Remarks: When a datasource is found that needs to be updated, a timestamp is
					immediately updated so that if queried again, that same result will
					not be returned.  It will not be returned for at least another hour.
					That also allows multiple processes to query the queue, and not
					interfere with each other.

Todo: Can we use a transaction to lock the table?

*/

Declare @ConfiguredDatasourceId Int

Select @ConfiguredDatasourceId = cd.Id
From ConfiguredDatasources cd
Left Outer Join RawData rd On cd.Id = rd.ConfiguredDatasourceId
Join Urls On Urls.Id = cd.UrlId
--Calculate if we're in a new time period
Where (rd.[Timestamp] Is Null Or DateAdd(d, cd.CheckFrequencyDays, rd.[Timestamp]) < GetUtcDate())
--Wait at least an hour before reattempting to try this datasource
And (cd.LastCheckAttempt Is Null Or DateAdd(d, 1, cd.LastCheckAttempt) < GetUtcDate())
And cd.Enabled = 1
And (@UserId Is Null Or Urls.UserId = @UserId)
--Make sure we're checking the time against the last raw data point
And (rd.[Timestamp] Is Null Or
			rd.[Timestamp] = (Select Max([Timestamp]) From RawData Where ConfiguredDatasourceId = cd.Id))
--Don't check generic datasources
And cd.DatasourceTypeId <> 8
Order By rd.[Timestamp] Desc

--Is this necessary?
--Order By DateAdd(d, cd.CheckFrequencyDays, rd.[Timestamp]) Asc

Update ConfiguredDatasources
Set LastCheckAttempt = GetUtcDate()
Where Id = @ConfiguredDatasourceId

Select @ConfiguredDatasourceId

Go

GRANT EXECUTE ON dbo.[Ds_GetNextConfiguredDatasourceId] TO [Public]
Go

/*

[Ds_GetNextConfiguredDatasourceId] '66F6A117-26EC-44D8-A35B-134D833ED5FC'
select * from ConfiguredDatasources
select * from rawdata
select * from urls

update configureddatasources
set lastcheckattempt = null

*/

