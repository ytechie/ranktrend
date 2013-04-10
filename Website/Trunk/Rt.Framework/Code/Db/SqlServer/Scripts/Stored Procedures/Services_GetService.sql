if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Services_GetService]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Services_GetService]
Go

CREATE PROCEDURE dbo.[Services_GetService]
  @ServiceId Int
AS

/*

Summary: Gets the information about a particular service from
			the services table.

*/

Select *, GetUtcDate() [ServerTime]
From Services
Where Id = @ServiceId

Go

GRANT EXECUTE ON dbo.[Services_GetService] TO [Public]
Go

/*
select * from services

[Services_GetService] 1

sp_columns services
*/

