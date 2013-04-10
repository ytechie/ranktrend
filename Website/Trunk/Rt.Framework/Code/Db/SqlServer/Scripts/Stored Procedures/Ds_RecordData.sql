if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_RecordData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_RecordData]
Go

CREATE PROCEDURE dbo.[Ds_RecordData]
	@ConfiguredDatasourceId Int,
	@FloatValue float,
	@Success bit --1 if the check was a success, otherwise 0
AS

/*

*/

If @Success Is Null
	Set @Success = 1

Insert Into RawData
(ConfiguredDatasourceId, [Timestamp], FloatValue, Success)
Select @ConfiguredDatasourceId, GetUTCDate(), @FloatValue, @Success

Go

GRANT EXECUTE ON dbo.[Ds_RecordData] TO [Public]
Go

/*

select * from configuredDatasources
select * from DatasourceParameters
select * from rawdata

Ds_GetParameters 1

*/

