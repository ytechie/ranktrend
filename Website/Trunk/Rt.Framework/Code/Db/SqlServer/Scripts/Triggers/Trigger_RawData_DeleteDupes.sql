IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'Trigger_RawData_DeleteDupes')
DROP TRIGGER Trigger_RawData_DeleteDupes
GO

CREATE TRIGGER dbo.Trigger_RawData_DeleteDupes ON RawData
INSTEAD OF INSERT
AS
BEGIN
	SET NOCOUNT ON

	Delete From RawData
	From RawData
	Inner Join inserted i On
		(RawData.ConfiguredDatasourceId = i.ConfiguredDatasourceId
		And ((RawData.DatasourceSubTypeId Is Null And i.DatasourceSubTypeId Is Null) Or RawData.DatasourceSubTypeId = i.DatasourceSubTypeId)
		And RawData.[Timestamp] = i.[Timestamp])

	Insert Into RawData (ConfiguredDatasourceId, DatasourceSubTypeId, [Timestamp], FloatValue, Success, Fuzzy)
	Select ConfiguredDatasourceId, DatasourceSubTypeId, [Timestamp], FloatValue, Success, Fuzzy From Inserted
END

/*

select * from rawdata

insert into rawdata
select 1, null, '2006-09-21 03:11:16.403', 2, 1, 0

*/