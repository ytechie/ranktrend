if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Report_SummaryView]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Report_SummaryView]
Go

CREATE PROCEDURE dbo.[Report_SummaryView]
	@SiteId Int
AS

Declare @Results Table
(
	ConfiguredDatasourceId Int,
	SubTypeId Int,
	DatasourceName Varchar(Max),
	LastTimestamp DateTime,
	LastValue Float,
	CurrentWeekAvg Float,
	Current30DayAvg Float,
	OverallAvg Float,
	StandardDeviation Float
)

Declare @CurrentCalculation Table
(
	ConfiguredDatasourceId Int,
	SubTypeId Int,
	CalculatedValue Float,
	CalculatedTimestamp DateTime
)

--Gather up the ID's to report on
Insert Into @Results
(ConfiguredDatasourceId, SubTypeId, DatasourceName)
Select cd.Id, st.Id, dbo.Ds_DatasourceDisplayName(cd.Id, st.Id)
From ConfiguredDatasources cd
Join DatasourceTypes types On types.Id = cd.DatasourceTypeId
Left Outer Join DatasourceSubTypes st On cd.DatasourceTypeId = st.DatasourceTypeId
Where UrlId = @SiteId

--select * from datasourcetypes

--Get the last timestamps for each datasource
Update @Results
Set LastTimestamp = [Timestamp]
From @Results r
Join RawData rd On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId And rd.DatasourceSubTypeId = r.SubTypeId)
Where rd.[Timestamp] = (Select Max([Timestamp]) From RawData Where ConfiguredDatasourceId = r.ConfiguredDatasourceId)

-------------------------------------
-- Current Week Averages
-------------------------------------
Delete From @CurrentCalculation

Insert Into @CurrentCalculation
Select rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId, Avg(rd.FloatValue), Null
From RawData rd
Join @Results r On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId))
Where rd.[Timestamp] Between DateAdd(dd, -7, GetUtcDate()) And GetUtcDate()
And rd.FloatValue Is Not Null
Group By rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId

Update @Results
Set CurrentWeekAvg = cc.CalculatedValue
From @Results r
Join @CurrentCalculation cc On (cc.ConfiguredDatasourceId = r.ConfiguredDatasourceId
								And ((cc.SubTypeId Is Null And r.SubTypeId Is Null) Or cc.SubTypeId = r.SubTypeId))

-------------------------------------
-- Current 30 Day Averages
-------------------------------------
Delete From @CurrentCalculation

Insert Into @CurrentCalculation
Select rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId, Avg(rd.FloatValue), Null
From RawData rd
Join @Results r On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId))
Where rd.[Timestamp] Between DateAdd(dd, -30, GetUtcDate()) And GetUtcDate()
And rd.FloatValue Is Not Null
Group By rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId

Update @Results
Set Current30DayAvg = cc.CalculatedValue
From @Results r
Join @CurrentCalculation cc On (cc.ConfiguredDatasourceId = r.ConfiguredDatasourceId
								And ((cc.SubTypeId Is Null And r.SubTypeId Is Null) Or cc.SubTypeId = r.SubTypeId))

-------------------------------------
-- Overall Averages
-------------------------------------
Delete From @CurrentCalculation

Insert Into @CurrentCalculation
Select rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId, Avg(rd.FloatValue), Null
From RawData rd
Join @Results r On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId))
Where rd.FloatValue Is Not Null
Group By rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId

Update @Results
Set OverallAvg = cc.CalculatedValue
From @Results r
Join @CurrentCalculation cc On (cc.ConfiguredDatasourceId = r.ConfiguredDatasourceId
								And ((cc.SubTypeId Is Null And r.SubTypeId Is Null) Or cc.SubTypeId = r.SubTypeId))

-------------------------------------
-- Standard Deviation
-------------------------------------
Delete From @CurrentCalculation

Insert Into @CurrentCalculation
Select rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId, StDev(rd.FloatValue), Null
From RawData rd
Join @Results r On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId))
Where rd.FloatValue Is Not Null
Group By rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId

Update @Results
Set StandardDeviation = cc.CalculatedValue
From @Results r
Join @CurrentCalculation cc On (cc.ConfiguredDatasourceId = r.ConfiguredDatasourceId
								And ((cc.SubTypeId Is Null And r.SubTypeId Is Null) Or cc.SubTypeId = r.SubTypeId))

-------------------------------------
-- Last Timestamp
-------------------------------------
Delete From @CurrentCalculation

Insert Into @CurrentCalculation
Select rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId, Null, Max(rd.[Timestamp])
From RawData rd
Join @Results r On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId))
Group By rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId

Update @Results
Set LastTimestamp = cc.CalculatedTimestamp
From @Results r
Join @CurrentCalculation cc On (cc.ConfiguredDatasourceId = r.ConfiguredDatasourceId
								And ((cc.SubTypeId Is Null And r.SubTypeId Is Null) Or cc.SubTypeId = r.SubTypeId))

-------------------------------------
-- Last Value
-------------------------------------
Delete From @CurrentCalculation

Insert Into @CurrentCalculation
Select rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId, Null, Max(rd.[Timestamp])
From RawData rd
Join @Results r On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId))
Group By rd.ConfiguredDatasourceId, rd.DatasourceSubTypeId

Update @Results
Set LastValue = rd.FloatValue
From @Results r
Join RawData rd On (rd.ConfiguredDatasourceId = r.ConfiguredDatasourceId
					And ((rd.DatasourceSubTypeId Is Null And r.SubTypeId Is Null) Or rd.DatasourceSubTypeId = r.SubTypeId)
					And rd.[Timestamp] = r.LastTimestamp)


Select *
From @Results

Go

GRANT EXECUTE ON dbo.[Report_SummaryView] TO [Public]
Go

/*
[Report_SummaryView] 3
[Report_SummaryView] '1-1-07', '1-20-97', 1
select * from urls

select * from configureddatasources
sp_columns configureddatasources
select * from datasourcesubtypes

select * from rawdata

sp_columns rawdata
*/

