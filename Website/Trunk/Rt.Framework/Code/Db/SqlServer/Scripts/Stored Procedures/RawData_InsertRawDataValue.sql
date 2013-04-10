if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RawData_InsertRawDataValue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[RawData_InsertRawDataValue]
GO

CREATE PROCEDURE dbo.[RawData_InsertRawDataValue]
	@ConfiguredDatasourceId Int,
	@DatasourceSubTypeId Int = Null,
	@Timestamp DateTime,
	@FloatValue Float = Null,
	@Success Bit,
	@Fuzzy Bit,
	@SourceData Varchar(Max) = Null
AS

Insert Into RawData
(ConfiguredDatasourceId, DatasourceSubTypeId, [Timestamp], FloatValue, Success, Fuzzy)
Values(@ConfiguredDatasourceId, @DatasourceSubTypeId, @Timestamp, @FloatValue,
	@Success, @Fuzzy)

--Note: I use @@Identity because Scope_Identity will return NULL because
--		there is an "instead of" trigger on RawData.

If(@SourceData Is Not Null)
	Insert Into rt_DatasourceDataQueue
	(Data, RawDataId, LastAttempt)
	Values(@SourceData, @@Identity, @Timestamp)

GO

GRANT EXECUTE ON dbo.[RawData_InsertRawDataValue] TO [Public]
Go

/*
Declare @CurrTime DateTime
Set @CurrTime = GetUtcDate()
exec RawData_InsertRawDataValue 1, Null, @CurrTime, 999, 1, 0, null

sp_columns rawdata;
go
sp_columns rt_DatasourceDataQueue;

select * from rt_datasourcedataqueue
select * from rawdata where configureddatasourceId = 1 and floatvalue = 999
*/

