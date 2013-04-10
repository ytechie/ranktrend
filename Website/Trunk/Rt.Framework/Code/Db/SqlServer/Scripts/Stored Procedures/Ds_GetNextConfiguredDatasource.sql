if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetNextConfiguredDatasource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetNextConfiguredDatasource]
Go

CREATE PROCEDURE dbo.[Ds_GetNextConfiguredDatasource]
AS

/*

Summary: Gets the configured data source that is most due for an update.  If no
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
--Calculate if we're in a new time period
Where (rd.[Timestamp] Is Null Or DateAdd(d, cd.CheckFrequencyDays, rd.[Timestamp]) < GetUtcDate())
--Wait at least an hour before reattempting to try this datasource
And (cd.LastCheckAttempt Is Null Or DateAdd(hh, 1, cd.LastCheckAttempt) < GetUtcDate())
And cd.Enabled = 1
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

GRANT EXECUTE ON dbo.[Ds_GetNextConfiguredDatasource] TO [Public]
Go

/*

[Ds_GetNextConfiguredDatasource]
select * from ConfiguredDatasources
select * from rawdata

*/

