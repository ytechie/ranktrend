if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Report_DetailView]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Report_DetailView]
Go

CREATE PROCEDURE dbo.[Report_DetailView]
	@ConfiguredDatasourceId Int = Null,
	@DatasourceSubTypeId Int = Null,
	@Start DateTime = Null,
	@End DateTime = Null
AS

Select dbo.Ds_DatasourceDisplayName(ConfiguredDatasourceId, DatasourceSubTypeId) DatasourceName,
	[Timestamp], FloatValue, Success, Fuzzy
From RawData
Where (@ConfiguredDatasourceId Is Null Or ConfiguredDatasourceId = @ConfiguredDatasourceId)
And (@DatasourceSubTypeId Is Null Or DatasourceSubTypeId = @DatasourceSubTypeId)
And (@Start Is Null Or [Timestamp] >= @Start)
And (@End Is Null Or [Timestamp] <= @End)
Order By ConfiguredDatasourceId, DatasourceSubTypeId, [Timestamp]

Go

GRANT EXECUTE ON dbo.[Report_DetailView] TO [Public]
Go

/*
[Report_DetailView] 33
[Report_DetailView] 33, 2
[Report_DetailView] 33, 2, '2005-06-16 00:00:00.000'
[Report_DetailView] 33, 2, '2005-06-16 00:00:00.000', '2005-06-20 00:00:00.000'
select * from urls

select * from configureddatasources
sp_columns configureddatasources
select * from datasourcesubtypes

select * from rawdata

sp_columns rawdata
*/

