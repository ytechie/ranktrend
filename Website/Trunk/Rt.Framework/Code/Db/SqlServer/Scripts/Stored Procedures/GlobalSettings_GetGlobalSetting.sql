if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetGlobalSetting]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetGlobalSetting]
GO

CREATE PROCEDURE dbo.GetGlobalSetting
  @GlobalSettingId Int = Null
AS

Select * From GlobalSettings Where Id = @GlobalSettingId

Go

GRANT EXECUTE ON dbo.GetGlobalSetting TO [Public]
Go


/*
GetGlobalSetting 2
select * from globalsettings

*/

