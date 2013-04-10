if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetTrendDatasourceList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetTrendDatasourceList]
Go

CREATE PROCEDURE dbo.[Ds_GetTrendDatasourceList]
	@UserId UniqueIdentifier
AS

/*

Summary: Gets a list of datasources that gets displayed in the
	interactive report list.  It does the work of expanding the
	datasource sub types into a simple datasource list.

*/

Select
	cd.Id, Null SubTypeId,
	--If the data type is generic, display the name, otherwise use the type
	dbo.Ds_DatasourceDisplayName(cd.Id, Null) 'Data Type',
	u.Url
From ConfiguredDatasources cd
Join DatasourceTypes types On cd.DatasourceTypeId = types.Id
Join Urls u On cd.UrlId = u.Id
--Exclude datasources with subtypes, since we're listing the subtypes
Where DatasourceTypeId Not In (Select DatasourceTypeId From DatasourceSubTypes)
And u.UserId = @UserId

Union

Select
	cd.Id, subTypes.Id SubTypeId,
	dbo.Ds_DatasourceDisplayName(cd.Id, subTypes.Id) 'Data Type', u.Url
From DatasourceSubTypes subTypes
Join ConfiguredDatasources cd On subTypes.DatasourceTypeId = cd.DatasourceTypeId
Join DatasourceTypes types On cd.DatasourceTypeId = types.Id
Join Urls u On cd.UrlId = u.Id
Where u.UserId = @UserId
Order By Url, [Data Type]

Go

GRANT EXECUTE ON dbo.[Ds_GetTrendDatasourceList] TO [Public]
Go

/*

exec [Ds_GetTrendDatasourceList] '66F6A117-26EC-44D8-A35B-134D833ED5FC'

select * from ConfiguredDatasources
select * from datasourcetypes
select * from datasourcesubtypes
select * from urls

*/

