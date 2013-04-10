if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetParameterList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetParameterList]
Go

CREATE PROCEDURE dbo.[Ds_GetParameterList]
	@DatasourceTypeId Int
AS

/*

Summary: Gets a list of all the parameters needed for a datasource

*/

Select *
From DatasourceParameterTypes types
Where types.DatasourceTypeId = @DatasourceTypeId
Order By ParameterNumber

Go

GRANT EXECUTE ON dbo.[Ds_GetParameterList] TO [Public]
Go

/*

select * from configuredDatasources
select * from DatasourceParameters
select * from datasourcetypes
select * from datasourceparametertypes

[Ds_GetParameterList] 2

*/

