if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetDatasourceParameterByDatasource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Ds_GetDatasourceParameterByDatasource]
GO

CREATE PROCEDURE dbo.[Ds_GetDatasourceParameterByDatasource]
	@ConfiguredDatasourceId Int,
	@ParameterNumber Int
AS

Select *
From DatasourceParameters
Where ConfiguredDatasourceId = @ConfiguredDatasourceId
And ParameterNumber = @ParameterNumber

GO

GRANT EXECUTE ON dbo.[Ds_GetDatasourceParameterByDatasource] TO [Public]
Go

/*
select * from datasourceparameters

exec Ds_GetDatasourceParameterByDatasource 2,1
*/

