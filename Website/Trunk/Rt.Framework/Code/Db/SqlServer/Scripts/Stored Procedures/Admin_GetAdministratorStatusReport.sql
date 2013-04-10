if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Admin_GetAdministratorStatusReport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Admin_GetAdministratorStatusReport]
Go

CREATE PROCEDURE dbo.[Admin_GetAdministratorStatusReport]
	@UserId As UniqueIdentifier = Null
AS

Select '# of Configured Datasources' Label, Count(*) [Numeric Value], Null [Text Value]
From ConfiguredDatasources

Union All

Select '# of Users' Label, Count(*), Null
From Aspnet_Membership am
Where am.ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'

Union All

Select '# of Active Users' Label, Count(*), Null
From Aspnet_Membership am
Where am.ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A' And IsApproved = 1

Union All

Select 'Newest User' Label, Null, am.Email
From Aspnet_Membership am
Where am.ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'
And CreateDate = (Select Max(CreateDate)
									From Aspnet_Membership
									Where ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A')

Union All

Select 'Number of Raw Data Points' Label, Count(*), Null
From RawData

Union All

Select 'Estimated Raw Data Growth / Day' Label,
	Count(*) / Avg(CheckFrequencyDays), Null
From ConfiguredDatasources cd
Where cd.Enabled = 1

Union All

Select 'Avg 7 Day Customer Growth / Day' Label,
	Count(*) / 7, Null
From Aspnet_Membership am
Where am.CreateDate >= DateAdd(dd, -7, GetUtcDate())
And am.ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'

Union All

Select 'Last 24 Hours Customer Growth' Label,
	Count(*), Null
From Aspnet_Membership am
Where am.CreateDate >= DateAdd(dd, -1, GetUtcDate())
And am.ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'

Union All

Select 'Last 24 Hours Login Count' Label,
	Count(*), Null
From Aspnet_Membership am
Where am.LastLoginDate >= DateAdd(dd, -1, GetUtcDate())
And am.ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'

--Datasource Stats
Select dt.Description, dt.Enabled, IsNull(se.[Name], 'N/A') 'Search Engine',
	Count(*) '# Configured',
	(Select Max([Timestamp])
		From RawData
		Join ConfiguredDatasources On ConfiguredDatasourceId = ConfiguredDatasources.Id
		Where ConfiguredDatasources.DatasourceTypeId = dt.Id
		And Success = 1) 'Last Valid Data',
	(Select Count(*)
		From RawData rd
		Join ConfiguredDatasources On ConfiguredDatasourceId = ConfiguredDatasources.Id
		Where ConfiguredDatasources.DatasourceTypeId = dt.Id
		And rd.Success = 0
		And rd.[Timestamp] >= DateAdd(dd, -7, GetUtcDate())) 'Failed Reads Last 7 Days',
	(Select Count(*)
		From RawData rd
		Join ConfiguredDatasources On ConfiguredDatasourceId = ConfiguredDatasources.Id
		Where ConfiguredDatasources.DatasourceTypeId = dt.Id
		And rd.Success = 0
		And rd.[Timestamp] >= DateAdd(dd, -1, GetUtcDate())) 'Failed Reads Last 24 hours',
	(Select Count(*)
		From RawData rd
		Join ConfiguredDatasources On ConfiguredDatasourceId = ConfiguredDatasources.Id
		Where ConfiguredDatasources.DatasourceTypeId = dt.Id
		And rd.Fuzzy = 1
		And rd.[Timestamp] >= DateAdd(dd, -1, GetUtcDate())) 'Fuzzy Reads Last 24 hours'
From DatasourceTypes dt
Left Outer Join SearchEngines se On se.Id = dt.SearchEngineId
Left Outer Join ConfiguredDatasources cd On cd.DatasourceTypeId = dt.Id
Group By dt.Description, dt.Enabled, IsNull(se.[Name], 'N/A'), dt.Id

Exec sp_SpaceUsed

Go

GRANT EXECUTE ON dbo.[Admin_GetAdministratorStatusReport] TO [Public]
Go

/*

exec [Admin_GetAdministratorStatusReport]

select * from aspnet_membership
select * from searchengines
select top 100 * from rawdata
select * from configureddatasources
sp_spaceused
*/

