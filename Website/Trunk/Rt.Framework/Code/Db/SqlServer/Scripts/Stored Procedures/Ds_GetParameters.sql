if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetParameters]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetParameters]
Go

CREATE PROCEDURE dbo.[Ds_GetParameters]
	@ConfiguredDatasourceId Int
AS

/*

Summary: Retrieves all of the parameters for the specified
configured datasource ID.

*/

Select *
From DatasourceParameters dp
Where dp.ConfiguredDatasourceId = @ConfiguredDatasourceId

Go

GRANT EXECUTE ON dbo.[Ds_GetParameters] TO [Public]
Go

/*

select * from configuredDatasources
select * from DatasourceParameters

Ds_GetParameters 1

*/

