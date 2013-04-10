if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_SaveRawData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_SaveRawData]
Go

CREATE PROCEDURE dbo.[Ds_SaveRawData]
	@ConfiguredDatasourceId Int,
	@DatasourceSubTypeId Int = Null,
	@Timestamp DateTime,
	@FloatValue Float = Null,
	@Success Bit,
	@Fuzzy Bit
AS

Insert Into RawData
Select @ConfiguredDatasourceId, @DatasourceSubTypeId, @Timestamp,
	@FloatValue, @Success, @Fuzzy

Go

GRANT EXECUTE ON dbo.[Ds_SaveRawData] TO [Public]
Go

/*
select * from rawdata
sp_columns rawdata

*/

