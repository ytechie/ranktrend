if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_QueryRawData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_QueryRawData]
Go

CREATE PROCEDURE dbo.[Ds_QueryRawData]
	@Start DateTime,
	@End DateTime,
	@ConfiguredDatasourceId Int,
	@DataSubTypeId Int = Null
AS

Select [Timestamp], FloatValue
From RawData
Where ConfiguredDatasourceId = @ConfiguredDatasourceId
And Success = 1
And [Timestamp] Between @Start And @End
And ((@DataSubTypeId Is Null And DatasourceSubTypeId Is Null)
			Or DatasourceSubTypeId = @DataSubTypeId)
--Order By [Timestamp]

--Note: We don't order by timestamp, because we're counting on them being
--		in order because of the clustered index (dangerous but VERY fast)

Go

GRANT EXECUTE ON dbo.[Ds_QueryRawData] TO [Public]
Go

/*
select * from rawdata where configureddatasourceId = 1
exec [Ds_QueryRawData] '2006-10-22 03:10:09.220', '2007-12-19 22:41:35.577', 3, null
exec [Ds_QueryRawData] '2006-10-22 03:10:09.220', '2007-12-19 22:41:35.577', 33, 1
--this should give no data
exec [Ds_QueryRawData] '2006-10-22 03:10:09.220', '2007-12-19 22:41:35.577', 33, null

exec Ds_QueryRawData @Start='2007-07-19 00:00:00:000',@End='2008-07-19 00:00:00:000',@ConfiguredDatasourceId=429,@DataSubTypeId=default
*/

